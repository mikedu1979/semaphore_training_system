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

namespace semaphore_training_system
{
    public partial class compareWindow : Window
    {
        public compareWindow(char[] show,char[] record)
        {
            InitializeComponent();

            string showStr=new string (show);
            rightMasssge.Text = showStr;            
            
            for (int i = 0; i < showStr.Length; i++)
            {
                if (show[i] == ' ' || show[i] == '.')
                {
                    record[i] = show[i];
                }
            }

            int wrongMassageGroup = 0;
            int finalScore = 100;
            bool wrongChar = false;
            bool spaceOccure = false;

            for (int i = 0; i < showStr.Length; i++)
            { 
                string str=record[i].ToString();

                Run run = new Run(str);
                run.Foreground = Brushes.Green;
                
                if (record[i] != show[i])
                {
                    wrongChar = true;
                    run.Foreground = Brushes.Red;
       
                }

                if (show[i] == ' ' || show[i] == '.')
                {
                    if (wrongChar == true)
                    {
                        wrongMassageGroup++;
                        wrongChar = false;
                    }
                }

                recordMasssge.Inlines.Add(run);
            }

            finalScore -= 35 * wrongMassageGroup;
            if (finalScore < 0) finalScore = 0;

            wrongTime.Content = "发错：  "+wrongMassageGroup+"  组";
            score.Content = "得分： " + finalScore + " 分";  
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
