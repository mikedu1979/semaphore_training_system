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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Kinect;
using System.Runtime.InteropServices;
using System.IO;

namespace semaphore_training_system
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        KinectSensor kinect;

        bool isWindowsClosing = false;
        bool aaaa11temp = false;
        bool aaa11temp = false;
        int aaa1temp = 0; 
        int aaaa111temp = 0;
        int totalMassageLines = 0;
        int aaatemp = 6;
        char[] aatmp;
        char[] aatmp1;
        int aaatmp = 0;
        char[] aatemp;
        List<string> txtMassageFile = new List<string>(); 
        int atemp = 0;
        bool massageSetOK = false;
        int[] atmp = new int[60];

        private void startKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinect = KinectSensor.KinectSensors[0];
                if (kinect == null)
                    return;

                var tsp = new TransformSmoothParameters
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };

                kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinect.SkeletonStream.Enable(tsp);

                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
                kinect.Start();
            }
            else
            {
                int screenW = (int)SystemParameters.PrimaryScreenWidth;
                int screenH = (int)SystemParameters.PrimaryScreenHeight;
            }
        }

        bool useOnlyOnce = true;
        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if(colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;

                colorFrameShow.Source =
                    BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                    96, 96, PixelFormats.Bgr32, null, pixels, stride);

                aaa1temp++;

                testLabel.Content = "当前报文编号：" + currentFileNo_str + "         " + aaa1temp + '\n' + "当前报文速度：" + currentShowSpeed + "码/分";
                if (aaa1temp == 30)
                {
                    aaa1temp = 0;
                    aaaa111temp++;

                    if (aaaa111temp == 10000)
                        aaaa111temp = 0;

                    if (aaaa11temp == true)
                    {
                        if (aaatemp > 0)
                        {
                            symbolShowLabel.FontSize = 600;
                            symbolShowLabel.FontFamily = new System.Windows.Media.FontFamily("new time romance");

                            aaatemp--;
                            symbolShowLabel.Content = aaatemp;

                            aaaa111temp = 0;
                            if (aaatemp <= 0)
                            {
                                aaaa111temp = 1;

                                symbolShowLabel.Content = aatmp[aaatmp];
                                testLabel1.Width = 1;
                            }

                            currentMassage.Text = massageStr;
                        }
                        else
                        {
                            if (aaaa111temp % currentShowSpeedTransTime == 0 && aaatmp < aatmp.Length)
                            {
                                testLabel1.Width = 1;
                                recogCount = 0;
                                for (int i = 0; i < 60; i++)
                                {
                                    atmp[i] = 0;
                                }

                                aaatmp++;

                                char showSymbol = aatmp[aaatmp];

                                if (showSymbol == ' ' || showSymbol == '’')
                                {
                                    symbolShowLabel.FontSize = 600;
                                    symbolShowLabel.FontFamily = new System.Windows.Media.FontFamily("黑体");
                                    symbolShowLabel.Content = "隔音";


                                    if (showSymbol == ' ' && aaa11temp == true)
                                    {
                                        aaaa11temp = false;
                                        aaatemp = 5;
                                        pause.Content = "继    续";
                                        currentMassage.Text = "";

                                        pause.IsEnabled = true;
                                    }
                                }
                                else if (showSymbol == '.')
                                {
                                    symbolShowLabel.FontSize = 600;
                                    symbolShowLabel.FontFamily = new System.Windows.Media.FontFamily("黑体");

                                    symbolShowLabel.Content = "完毕";
                                    aaaa11temp = false;
                                    aaa1temp = 0;
                                    aaaa111temp = 0;
                                    aaatmp = 0;

                                    kinect.Stop();

                                    messageCompare.IsEnabled = true;
                                    exit.IsEnabled = true;
                                }
                                else
                                {
                                    symbolShowLabel.FontSize = 600;
                                    symbolShowLabel.FontFamily = new System.Windows.Media.FontFamily("new time romance");

                                    symbolShowLabel.Content = aatmp[aaatmp];
                                }
                            }
                        }
                    }
                }
            }
        }

        int recogCount = 0;

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            if (isWindowsClosing)
            {
                return;
            }

            Skeleton s = GetClosetSkeleton(e);

            if (s == null)
            {
                skeletonTrackingState.Content = "骨架跟踪状态：未跟踪";
                skeletonTrackingState.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }

            if (s.TrackingState != SkeletonTrackingState.Tracked)
            {
                skeletonTrackingState.Content = "骨架跟踪状态：未跟踪";
                skeletonTrackingState.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                skeletonTrackingState.Content = "骨架跟踪状态：正在跟踪";

                char recogResult = gestureMatch(s, aatmp[aaatmp]);

                if (recogResult >= 'A' && recogResult <= 'Z')
                {
                    atmp[recogResult - 'A']++;
                }
                for (int i = 0; i < 30; i++)
                {
                    if (atmp[i] == currentGestureConfirmTime)
                    {
                        string ccc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ'+++++++++++++++++";
                        char c = ccc[i];
                        aatemp[aaatmp] = c;
                    }
                }

                if (recogResult == aatmp[aaatmp])
                {
                    if (recogCount <= currentGestureConfirmTime)
                    {
                        recogCount++;
                        testLabel1.Width = 3 * recogCount * 60.0 / (currentGestureConfirmTime * 1.0);
                        aatemp[aaatmp] = aatmp[aaatmp];
                    }
                }
            }

            if (aaaa11temp == true)
            {
            }
        }

        private char gestureMatch(Skeleton s, char currentChar)
        {
            Joint rightHand = s.Joints[JointType.HandRight];
            Joint leftHand = s.Joints[JointType.HandLeft];
            Joint head = s.Joints[JointType.Head];
            Joint leftShoulder = s.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = s.Joints[JointType.ShoulderRight];

            int LH_LS_A = 0;
            int RH_RS_A = 0;

            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);

            p1.X = rightHand.Position.X;
            p1.Y = rightHand.Position.Y;
            p2.X = rightShoulder.Position.X;
            p2.Y = rightShoulder.Position.Y;
            RH_RS_A = (int)(Math.Acos((p1.X - p2.X) / Math.Sqrt(Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.X - p2.X, 2))) / 3.14159 * 180);

            p1.X = leftHand.Position.X;
            p1.Y = leftHand.Position.Y;
            p2.X = leftShoulder.Position.X;
            p2.Y = leftShoulder.Position.Y;
            LH_LS_A = (int)(Math.Acos((p1.X - p2.X) / Math.Sqrt(Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.X - p2.X, 2))) / 3.14159 * 180);

            LH_LS_A = 180 - LH_LS_A;

            if (LH_LS_A < 15) LH_LS_A = 0;
            else if (LH_LS_A > 30 && LH_LS_A < 60) LH_LS_A = 45;
            else if (LH_LS_A > 75 && LH_LS_A < 105) LH_LS_A = 90;
            else if (LH_LS_A > 120 && LH_LS_A < 150) LH_LS_A = 135;
            else if (LH_LS_A > 165 && LH_LS_A < 195) LH_LS_A = 180;
            else LH_LS_A = 1000;

            if (RH_RS_A < 15) RH_RS_A = 0;
            else if (RH_RS_A > 30 && RH_RS_A < 60) RH_RS_A = 45;
            else if (RH_RS_A > 75 && RH_RS_A < 105) RH_RS_A = 90;
            else if (RH_RS_A > 120 && RH_RS_A < 150) RH_RS_A = 135;
            else if (RH_RS_A > 165 && RH_RS_A < 195) RH_RS_A = 180;
            else RH_RS_A = 1000;

            if (leftHand.Position.Y < leftShoulder.Position.Y)
                LH_LS_A = -LH_LS_A;
            if (rightHand.Position.Y < rightShoulder.Position.Y)
                RH_RS_A = -RH_RS_A;

            if (Math.Abs(LH_LS_A) == 180) LH_LS_A = 180;
            if (Math.Abs(RH_RS_A) == 180) RH_RS_A = 180;

            angleLabel.Content = LH_LS_A + "    " + RH_RS_A;

            char recogChar = '+';

            for (int i = 0; i < poseConfiguration.leftHandAngle.Length - 1; i++)
            {
                if (LH_LS_A == poseConfiguration.leftHandAngle[i] && RH_RS_A == poseConfiguration.rightHandAngle[i])
                {
                    recogChar = poseConfiguration.character[i];
                    break;
                }
            }

            char result = 'F';
            if (recogChar == currentChar)
                result = 'T';

            return recogChar;
        }

        const int MaxSkeletonTrackingCount = 6;
        Skeleton[] allSkeletons = new Skeleton[MaxSkeletonTrackingCount];

        Skeleton GetClosetSkeleton(SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                Skeleton closestSkeleton = (from s in allSkeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked &&
                                                  s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                                            select s).OrderBy(s => s.Joints[JointType.Head].Position.Z)
                                    .FirstOrDefault();

                return closestSkeleton;
            }
        }

        private void stopKinect()
        {
            if (kinect != null)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    kinect.Stop();
                }
            }
        }

        int currentFileNo = 0;
        int currentShowSpeed = 1;
        int currentGestureConfirmTime = 60;

        int currentShowSpeedTransTime = 1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtMassageFile = readTxtMassage();
            atemp = txtMassageFile.Count;

            symbolShowLabel.FontFamily = new System.Windows.Media.FontFamily("黑体");
            symbolShowLabel.Content = "手旗发信训练系统";
            symbolShowLabel.FontSize = 70;

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectConnectState.Content = "硬件连接结果：Kinect已连接";
            }
            else
            {
                kinectConnectState.Content = "硬件连接结果：Kinect未连接";
                kinectConnectState.Foreground = new SolidColorBrush(Colors.Red);
                MessageBoxResult result = MessageBox.Show("未检测到硬件设备，请连接Kinect(体感摄像头)并重启程序！");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isWindowsClosing = true;
            stopKinect();
        }

        setWindow massageSetWin;
        private void messageSet_Click(object sender, RoutedEventArgs e)
        {
            massageSetWin = new setWindow(totalMassageLines);

            massageSetWin.Show();
            massageSetWin.ReturnSetDataEvent += new ReturnSetDataHandler(settingDataReceiver);
        }

        string massageStr = " ";
        string currentFileNo_str = "";

        void settingDataReceiver(int fileNo_, int showSpeed_, double gestureConfirmTime_)
        {
            massageSetOK = true;

            currentFileNo = fileNo_ - 1;
            currentShowSpeed = showSpeed_;
            currentGestureConfirmTime = (int)(gestureConfirmTime_ * 30.0);

            currentShowSpeedTransTime = 60 / currentShowSpeed;

            if (currentFileNo < 10) currentFileNo_str = "000" + currentFileNo;
            else if (currentFileNo < 100) currentFileNo_str = "00" + currentFileNo;
            else if (currentFileNo < 1000) currentFileNo_str = "0" + currentFileNo;
            else currentFileNo_str = "" + currentFileNo;

            testLabel.Content = "当前报文编号：" + currentFileNo_str + '\n' + "当前报文速度：" + currentShowSpeed + "码/分";

            char[] aatmpTmp = new char[200];
            aatmpTmp = txtMassageFile[currentFileNo].ToCharArray();

            int charLength = txtMassageFile[currentFileNo].Length;

            aatmp1 = new char[charLength - 6];

            for (int i = 0, j = 6; j < charLength; i++, j++)
            {
                aatmp1[i] = aatmpTmp[j];
            }

            char[] aatmpTmp1 = new char[200];
            int lengthTmp = 0;
            int[] spellCount = { 4, 3, 5, 3, 2, 2, 3, 4, 2, 3, 3 };
            for (int i = 0, j = 0; j < txtMassageFile[currentFileNo].Length - 6; j++)
            {
                char c = aatmp1[j];
                if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
                {
                    for (int k = 0; k < spellCount[c - 48]; k++)
                    {
                        aatmpTmp1[i] = transDigit2Char(c - 48)[k];

                        string s;
                        s = transDigit2Char(c - 64).ToString();

                        i++;
                    }
                }
                else
                {
                    aatmpTmp1[i] = c;
                    i++;
                }
                lengthTmp = i;
            }

            aatmp = new char[lengthTmp];
            aatemp = new char[lengthTmp];

            for (int i = 0; i < aatemp.Length; i++)
            {
                aatmp[i] = aatmpTmp1[i];
                aatemp[i] = '+';
            }

            massageStr = new string(aatmp1);
            currentMassage.Text = massageStr;
        }

        public char[] transDigit2Char(int digit)
        {
            char[] digit2Char = new char[6];
            string str_tmp;

            switch (digit)
            {
                case 0: str_tmp = "DONG"; digit2Char = str_tmp.ToCharArray(); break;
                case 1: str_tmp = "YAO"; digit2Char = str_tmp.ToCharArray(); break;
                case 2: str_tmp = "LIANG"; digit2Char = str_tmp.ToCharArray(); break;
                case 3: str_tmp = "SAN"; digit2Char = str_tmp.ToCharArray(); break;
                case 4: str_tmp = "SI"; digit2Char = str_tmp.ToCharArray(); break;
                case 5: str_tmp = "WU"; digit2Char = str_tmp.ToCharArray(); break;
                case 6: str_tmp = "LIU"; digit2Char = str_tmp.ToCharArray(); break;
                case 7: str_tmp = "GUAI"; digit2Char = str_tmp.ToCharArray(); break;
                case 8: str_tmp = "BA"; digit2Char = str_tmp.ToCharArray(); break;
                case 9: str_tmp = "GOU"; digit2Char = str_tmp.ToCharArray(); break;
            }
            return digit2Char;
        }

        public List<string> readTxtMassage()
        {
            List<string> txtMassageFile = new List<string>();
            string systemStr = System.IO.Directory.GetCurrentDirectory();
            string txtFileName = systemStr + @"\massageFile\massage.txt";

            using (StreamReader sr = new StreamReader(txtFileName, Encoding.Default))
            {
                int lineCount = 0;
                while (sr.Peek() > 0)
                {
                    lineCount++;
                    string temp = sr.ReadLine();
                    txtMassageFile.Add(temp);
                    totalMassageLines = lineCount;
                }
            }
            return txtMassageFile;
        }

        bool kinectFirstStart = true;
        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (massageSetOK == false)
            {
                MessageBoxResult result = MessageBox.Show("请设置报文！");
            }
            else
            {
                messageCompare.IsEnabled = false;
                exit.IsEnabled = false;

                if (aaaa11temp == false)
                {
                    aaaa11temp = true;

                    if (kinectFirstStart == true)
                    {
                        startKinect();
                        kinectFirstStart = false;
                    }
                    else
                    {
                        kinect.Start();
                    }

                    aaatemp = 6;
                }
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (aaaa11temp == true)
            {
                aaa11temp = true;
                pause.Content = "暂    停";

                pause.IsEnabled = false;
                start.IsEnabled = false;
            }
            else
            {
                aaaa11temp = true;
                aaa11temp = false;

                pause.Content = "暂    停";
                start.IsEnabled = true;
            }
        }
        
        compareWindow compareWin;
        private void messageCompare_Click(object sender, RoutedEventArgs e)
        {
            compareWin = new compareWindow(aatmp, aatemp);
            compareWin.Show();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认关闭此程序?","确认信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                isWindowsClosing = true;
                stopKinect();
                this.Close();
            }
            else
            {
            }
        }
    }
}
