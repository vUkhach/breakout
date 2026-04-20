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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// private DispatcherTimer gameTimer;
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
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnWindowLoaded; 
            this.MouseMove += OnMouseMove;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            double canvasHeight = GameCanvas.ActualHeight;
            double canvasWidth = GameCanvas.ActualWidth;

            paddle = new Paddle(canvasWidth / 2 - 50, canvasHeight - 40);

            CreateBlocks();
            SpawnBall();
            UpdateLivesPanel();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            ball.Move();
            
            CheckWallCollision();
            CheckPaddleCollision();
            CheckBlockCollision();

            DrawBall();
            DrawPaddle();
            DrawBlocks();
            ScoreText.Text = $"{score}";
            SpeedText.Text = $"{currentSpeed}";
            CheckWin();
            CheckLoose();
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

        private void SpawnBall()
        {
            double startX = paddle.X + paddle.Width / 2;
            double startY = paddle.Y - 15;
            Random rand = new Random();
            double dx = rand.NextDouble() * 4 - 2;

            ball = new Ball(startX, startY);

            ball.SetDirection(dx, -3);
        }

        private void CheckWallCollision()
        {
            double width = GameCanvas.ActualWidth;
            double height = GameCanvas.ActualHeight;

            if (ball.X <=0 || ball.X + ball.Size >=width) ball.BounceX();

            if (ball.Y <= 0) ball.BounceY();

            if(ball.Y + ball.Size >= height)
            {
                //ball.BounceY();
            }
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
            foreach (var block in blocks)
            {
                if (block.IsDestroyed) continue;

                bool collision = 
                    ball.X + ball.Size >= block.X && 
                    ball.X <= block.X + block.Width && 
                    ball.Y + ball.Size >= block.Y && 
                    ball.Y <= block.Y + block.Height;

                if (collision)
                {
                    block.Destroy();
                    score += 10;
                    ball.BounceY();

                    if (score % 50 == 0 && score != 0)
                    {
                        ball.IncreaseSpeed(ball.GetSpeed() + 1);
                        currentSpeed += 1;
                    }

                    break;
                }

                
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            double mouseX = e.GetPosition(GameCanvas).X;
            paddle.MoveTo(mouseX, GameCanvas.ActualWidth);
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

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    Block block = new Block(startX + j * (blockWidth + paddingX), startY + i * (blockHeight + paddingY));
                    blocks.Add(block);

                    Rectangle rect = new Rectangle { Width = block.Width, Height = block.Height, Fill = Brushes.White, RadiusX = 4, RadiusY = 4 };
                    Canvas.SetLeft(rect, block.X);
                    Canvas.SetTop(rect, block.Y);
                    blockShapes.Add(rect);
                    GameCanvas.Children.Add(rect);
                }
            }
        }

        private void DrawBlocks()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                var shape = blockShapes[i];

                if (block.IsDestroyed)
                {
                    shape.Visibility = Visibility.Hidden;
                    continue;
                }

                shape.Visibility = Visibility.Visible;

                Canvas.SetLeft(shape, block.X);
                Canvas.SetTop(shape, block.Y);
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
            bool allDestroyed = blocks.All(b => b.IsDestroyed);

            if (allDestroyed)
            {
                gameTimer.Stop();
                MessageBox.Show("You win");
                RestartGame();
                gameTimer.Start();
                return;
            }
        }

        private void CheckLoose()
        {
            if (ball == null) return;

            if(ball.Y > GameCanvas.ActualHeight)
            {
                lives--;
                LivesText.Text = $"{lives}";
                UpdateLivesPanel();

                if (lives <= 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show($"Game over! Your score: {score}");
                    lives = 3;
                    RestartGame();
                    gameTimer.Start();
                }
                else
                {
                    SpawnBall();
                }
            }
        }

        private void RestartGame()
        {
            score = 0;

            blocks.Clear();
            blockShapes.Clear();
            GameCanvas.Children.Clear();

            ballShape = null;
            paddleShape = null;
            currentSpeed = 1;

            CreateBlocks();
            SpawnBall();
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
    }
}