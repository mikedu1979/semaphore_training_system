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
    public delegate void ReturnSetDataHandler(int a, int b, double c);

    public partial class setWindow : Window
    {
        public setWindow(int totalmassageline)
        {
            totalMassageLines = totalmassageline;
            InitializeComponent();
        }

        public event ReturnSetDataHandler ReturnSetDataEvent;

        int fileNo = 0;
        int showSpeed = 0;
        double gestureConfirmTime = 0.0;
        public int totalMassageLines { get; }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            fileNo = Convert.ToInt32(textFileNo.Text);
            showSpeed = Convert.ToInt32(textShowSpeeed.Text);
            gestureConfirmTime = Convert.ToDouble(textGestureConfirmTime.Text);

            if (fileNo > totalMassageLines)
            {
                MessageBoxResult result = MessageBox.Show("报文编号超出范围，请重新输入！");
            }
            else if (60.0 / showSpeed < gestureConfirmTime)
            {
                MessageBoxResult result = MessageBox.Show("动作保持时间不能大于字码保持时间！");
            }
            else
            {
                ReturnSetDataEvent(fileNo, showSpeed, gestureConfirmTime);
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
