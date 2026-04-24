using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace breakout
{
    /// <summary>
    /// Interaction logic for GameOverWindow.xaml
    /// </summary>
    public partial class GameOverWindow : Window
    {
        public bool Restart { get; private set; }

        public GameOverWindow(int score)
        {
            InitializeComponent();
            ResultText.Text = $"Your score: {score}";
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Restart = true;
            Close();
        }

        private void Table_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardWindow window = new LeaderboardWindow();
            window.ShowDialog();
        }
    }
}
