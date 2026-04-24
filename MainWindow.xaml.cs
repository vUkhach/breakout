using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace breakout
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;
        private Ball ball;
        private Ellipse ballShape;
        private Paddle paddle;
        private Rectangle paddleShape;
        private List<Block> blocks = new List<Block>();
        private List<Rectangle> blockShapes = new List<Rectangle>();
        private int score = 0;
        private int lives = 3;
        private double currentSpeed = 1;
        private double gameTime = 0;
        private double canvasWidth;
        private double canvasHeight;
        private int destroyedCount = 0;
        private TimeSpan lastRenderTime = TimeSpan.Zero;
        public MainWindow()
        {
            InitializeComponent();

            using (var db = new GameDbContext())
            {
                db.Database.EnsureCreated();
            }

            this.Loaded += OnWindowLoaded; 
            this.MouseMove += OnMouseMove;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            canvasWidth = GameCanvas.ActualWidth;
            canvasHeight = GameCanvas.ActualHeight;

            paddle = new Paddle(canvasWidth / 2 - 50, canvasHeight - 40);

            CreateBlocks();
            SpawnBall(3);
            UpdateLivesPanel();

            CompositionTarget.Rendering += GameLoop;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            var args = (RenderingEventArgs)e;

            if (lastRenderTime == TimeSpan.Zero)
            {
                lastRenderTime = args.RenderingTime;
                return;
            }

            double delta = (args.RenderingTime - lastRenderTime).TotalSeconds;
            lastRenderTime = args.RenderingTime;

            if (delta > 0.05) delta = 0.05;

            ball.Move(delta);

            CheckWallCollision();
            CheckPaddleCollision();
            CheckBlockCollision();

            DrawBall();
            DrawPaddle();
            DrawBlocks();
            CheckWin();
            CheckLoose();
            gameTime += (int)(delta * 1000);
        }

        private void DrawBall()
        {
            if (ballShape == null)
            {
                ballShape = new Ellipse
                {
                    Width = ball.Size,
                    Height = ball.Size,
                    Fill = Brushes.White
                };

                GameCanvas.Children.Add(ballShape);
            }

            Canvas.SetLeft(ballShape, ball.X);
            Canvas.SetTop(ballShape, ball.Y);
        }

        private void SpawnBall(double speed)
        {
            double startX = paddle.X + paddle.Width / 2;
            double startY = paddle.Y - 15;
            Random rand = new Random();
            double dx = rand.NextDouble() * 4 - 2;
            double dy = -1;

            ball = new Ball(startX, startY);

            double length = Math.Sqrt(dx * dx + dy * dy);

            dx = (dx / length) * speed;
            dy = (dy / length) * speed;

            ball.SetDirection(dx, dy);
        }

        private void CheckWallCollision()
        {
            if (ball.X <= 0 || ball.X + ball.Size >= canvasWidth) ball.BounceX();
            if (ball.Y <= 0) ball.BounceY();
        }

        private void CheckPaddleCollision()
        {
            double ballBottom = ball.Y + ball.Size;
            double paddleTop = paddle.Y;

            double ballLeft = ball.X;
            double ballRight = ball.X + ball.Size;

            double paddleLeft = paddle.X;
            double paddleRight = paddle.X + paddle.Width;

            if (ballBottom >= paddleTop &&
                ball.Y <= paddleTop + paddle.Height &&
                ballRight >= paddleLeft &&
                ballLeft <= paddleRight) 
            {
                HandleCollision(paddle.X, paddle.Y, paddle.Width, paddle.Height, true);
                ball.SetPosition(ball.X, paddle.Y - ball.Size);
            }
        }

        private void CheckBlockCollision()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                if (block.IsDestroyed) continue;

                bool collision =
                    ball.X + ball.Size >= block.X &&
                    ball.X <= block.X + block.Width &&
                    ball.Y + ball.Size >= block.Y &&
                    ball.Y <= block.Y + block.Height;

                if (collision)
                {
                    block.Hit();
                    score += 10;
                    UpdateUI();
                    ball.BounceY();
                    if (block.IsDestroyed)
                        destroyedCount++;

                    if (block.Health == 2) blockShapes[i].Fill = Brushes.Orange;
                    else if (block.Health == 1) blockShapes[i].Fill = Brushes.White;
                    else if (block.IsDestroyed) blockShapes[i].Visibility = Visibility.Hidden;

                    if (score % 50 == 0 && score != 0)
                    {
                        ball.IncreaseSpeed(ball.GetSpeed() + 1);
                        currentSpeed += 1;
                        UpdateUI();
                    }

                    break;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            double mouseX = e.GetPosition(GameCanvas).X;
            paddle.MoveTo(mouseX, canvasWidth);
        }

        private void DrawPaddle()
        {
            if(paddleShape == null)
            {
                paddleShape = new Rectangle
                {
                    Width = paddle.Width,
                    Height = paddle.Height,
                    Fill = Brushes.White,
                    RadiusX = 5,
                    RadiusY = 5
                };

                GameCanvas.Children.Add(paddleShape);
            }

            Canvas.SetLeft(paddleShape, paddle.X);
            Canvas.SetTop(paddleShape, paddle.Y);
        }

        private void CreateBlocks()
        {
            int rows = 5;
            int cols = 10;

            double blockWidth = 70;
            double blockHeight = 25;
            double paddingX = 5;
            double paddingY = 5;
            double startX = 15;
            double startY = 15;

            Random rand = new Random();

            List<int> indices = new List<int>();
            int total = rows * cols;

            for (int i = 0; i < total; i++)
                indices.Add(i);

            indices = indices.OrderBy(x => rand.Next()).ToList();

            var hp2 = indices.Take(7).ToHashSet();
            var hp3 = indices.Skip(7).Take(5).ToHashSet();

            int index = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int health = 1;

                    if (hp3.Contains(index))
                        health = 3;
                    else if (hp2.Contains(index))
                        health = 2;

                    Block block = new Block(
                        startX + j * (blockWidth + paddingX),
                        startY + i * (blockHeight + paddingY),
                        health
                    );

                    blocks.Add(block);

                    Rectangle rect = new Rectangle
                    {
                        Width = block.Width,
                        Height = block.Height,
                        RadiusX = 4,
                        RadiusY = 4
                    };

                    if (health == 3)
                        rect.Fill = Brushes.Red;
                    else if (health == 2)
                        rect.Fill = Brushes.Orange;
                    else
                        rect.Fill = Brushes.White;

                    Canvas.SetLeft(rect, block.X);
                    Canvas.SetTop(rect, block.Y);

                    blockShapes.Add(rect);
                    GameCanvas.Children.Add(rect);

                    index++;
                }
            }
        }

        private void DrawBlocks()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].IsDestroyed)
                    blockShapes[i].Visibility = Visibility.Hidden;
            }
        }

        private void HandleCollision(double objX, double objY, double objW, double objH, bool isPaddle)
        {
            double ballCenterX = ball.X + ball.Size / 2;
            double ballCenterY = ball.Y + ball.Size / 2;

            double objCenterX = objX + objW / 2;
            double objCenterY = objY + objH / 2;

            double dx = ballCenterX - objCenterX;
            double dy = ballCenterY - objCenterY;

            if (isPaddle)
            {
                double speed = ball.GetSpeed();

                double relative = (ballCenterX - objX) / objW;

                double newDx = (relative - 0.5) * 2; 
                double newDy = -1;

                double length = Math.Sqrt(newDx * newDx + newDy * newDy);

                newDx = (newDx / length) * speed;
                newDy = (newDy / length) * speed;

                ball.SetDirection(newDx, newDy);
            }
            else
            {
                if (Math.Abs(dx) > Math.Abs(dy))
                    ball.BounceX();
                else
                    ball.BounceY();
            }
        }

        private void CheckWin()
        {
            if (destroyedCount >= blocks.Count)
            {
                CompositionTarget.Rendering -= GameLoop;

                var window = new GameOverWindow(score);
                this.Hide();
                window.ShowDialog();

                if (window.Restart)
                {
                    this.Show();
                    RestartGame();
                    CompositionTarget.Rendering += GameLoop;
                }
                return;
            }
        }

        private void CheckLoose()
        {
            double speed = ball.GetSpeed();
            if (ball == null) return;

            if(ball.Y > canvasHeight)
            {
                lives--;
                UpdateUI();
                LivesText.Text = $"{lives}";
                UpdateLivesPanel();

                if (lives <= 0)
                {
                    CompositionTarget.Rendering -= GameLoop;
                    SaveResult();
                    var window = new GameOverWindow(score);
                    this.Hide();
                    window.ShowDialog();
                    if (window.Restart)
                    {
                        this.Show();
                        RestartGame();
                        CompositionTarget.Rendering += GameLoop;
                    }
                }
                else
                {
                    SpawnBall(speed);
                }
            }
        }

        private void RestartGame()
        {
            score = 0;
            lives = 3;
            destroyedCount = 0;
            gameTime = 0;
            blocks.Clear();
            blockShapes.Clear();
            GameCanvas.Children.Clear();

            ballShape = null;
            paddleShape = null;
            currentSpeed = 1;
            

            CreateBlocks();
            SpawnBall(3);

            UpdateLivesPanel();
        }

        private void UpdateLivesPanel()
        {
            LivesPanel.Children.Clear();
            for (int i = 0; i < 3; i++)
            {
                Ellipse heart = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Margin = new Thickness(4, 0, 4, 0),
                    Fill = i < lives ? Brushes.White : new SolidColorBrush(Color.FromRgb(40, 40, 40))
                };
                LivesPanel.Children.Add(heart);
            }
        }

        private void SaveResult()
        {
            using (var db = new GameDbContext())
            {
                var result = new GameResult
                {
                    Score = score,
                    TimeSeconds = (int) (gameTime / 1000.0),
                    Date = DateTime.Now
                };

                db.Results.Add(result);
                db.SaveChanges();
            }
        }
        private void UpdateUI()
        {
            ScoreText.Text = $"{score}";
            SpeedText.Text = $"{currentSpeed}";
            LivesText.Text = $"{lives}";
        }
    }
}