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
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var db = new GameDbContext())
                {
                    var results = db.Results
                        .OrderByDescending(r => r.Score)
                        .ThenBy(r => r.TimeSeconds)
                        .Take(5)
                        .ToList();

                    if (results.Count == 0)
                    {
                        ResultsGrid.ItemsSource = null;

                        MessageBox.Show("No results yet!", "Leaderboard");
                        return;
                    }

                    ResultsGrid.ItemsSource = results;
                }
            }
            catch (Exception)
            {
            }
        }

        
    }
}
