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
        private Paddle paddle = new Paddle(200, 400);
        private Rectangle paddleShape;
        private List<Block> blocks = new List<Block>();
        private List<Rectangle> blockShapes = new List<Rectangle>();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnWindowLoaded; 
            this.MouseMove += OnMouseMove;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            CreateBlocks();
            SpawnBall();

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
            double startY = paddle.Y - 10;
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
                ball.Y <= paddleTop + paddle.Heigth &&
                ballRight >= paddleLeft &&
                ballLeft <= paddleRight) 
            {
                ball.BounceY();
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
                    ball.BounceY();
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
                    Height = paddle.Heigth,
                    Fill = Brushes.White
                };

                GameCanvas.Children.Add(paddleShape);
            }

            Canvas.SetLeft(paddleShape, paddle.X);
            Canvas.SetTop(paddleShape, paddle.Y);
        }

        private void CreateBlocks()
        {
            int rows = 5;
            int cols = 8;

            double startX = 50;
            double startY = 50;

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    Block block = new Block(startX + j * 70, startY + i * 30);
                    blocks.Add(block);

                    Rectangle rect = new Rectangle
                    {
                        Width = block.Width,
                        Height = block.Height,
                        Fill = Brushes.LightBlue
                    };
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
    }
}