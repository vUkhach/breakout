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
    public partial class StartMenuWindow : Window
    {
        public StartMenuWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MainWindow game = new MainWindow();
            game.Show();
            this.Close();
        }

        private void Leaderboard_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardWindow window = new LeaderboardWindow();
            window.ShowDialog();
        }
    }
}
