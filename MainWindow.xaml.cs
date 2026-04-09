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
        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        
        }

        private void GameLoop(object sender, EventArgs e)
        {
            ball.Move();
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
    }
}