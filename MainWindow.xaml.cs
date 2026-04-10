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

namespace breakout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// private DispatcherTimer gameTimer;
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;
        private Ball ball = new Ball(100,100);
        private Ellipse ballShape;
        private Paddle paddle = new Paddle(200, 400);
        private Rectangle paddleShape;
        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            this.MouseMove += OnMouseMove;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            ball.Move();
            CheckWallCollision();

            DrawPaddle();
            DrawBall();
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
    }
}