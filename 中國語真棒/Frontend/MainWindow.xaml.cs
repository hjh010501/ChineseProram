﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NPOI.XSSF.UserModel;
using 中國語真棒.Backend;
using 中國語真棒.Frontend;
using System.Media;

namespace 中國語真棒
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class MainWindow : System.Windows.Window
    {

        Storyboard MenuOn;
        Storyboard MenuOff;

        String toyear;

        private bool Dragging;

        XSSFWorkbook wb;
        XSSFSheet sh;

        private Point MouseDownLocation;

        public MainWindow()
        {
            InitializeComponent();
            MenuOn = (Storyboard)FindResource("OpenDisplay");
            MenuOff = (Storyboard)FindResource("CloseDisplay");

            toyear = DateTime.Now.ToString("yyyy");

            if (Directory.Exists("database"))
            {
                selectban.SelectedIndex = 0;
            } else {
                Directory.CreateDirectory("database");
                Directory.CreateDirectory("database\\" + toyear);
                Directory.CreateDirectory("database\\" + toyear + "\\1");
                Directory.CreateDirectory("database\\" + toyear + "\\2");
                Directory.CreateDirectory("database\\" + toyear + "\\3");
                Directory.CreateDirectory("database\\" + toyear + "\\4");
                Directory.CreateDirectory("database\\" + toyear + "\\5");
                Directory.CreateDirectory("database\\" + toyear + "\\6");

                wb = new XSSFWorkbook();

                // create sheet

                for (int sheet = 1; sheet <= 6; sheet++)
                {


                sh = (XSSFSheet)wb.CreateSheet(sheet+"조");

                for (int i = 0; i < 15; i++)
                {
                    var r = sh.CreateRow(i);
                    for (int j = 0; j < 15; j++)
                    {
                        r.CreateCell(j);
                    }
                }
                    
                    sh.GetRow(1).GetCell(1).SetCellValue("조이름");
                    sh.GetRow(1).GetCell(2).SetCellValue("여기에 조이름을 적어주세요");
                    sh.GetRow(2).GetCell(1).SetCellValue("조원들 (1번은 조장입니다.)");
                    sh.GetRow(2).GetCell(2).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(3).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(4).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(5).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(6).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(7).SetCellValue("징징이");
                    sh.GetRow(2).GetCell(8).SetCellValue("징징이(없을 시 공백)");
                    sh.GetRow(3).GetCell(1).SetCellValue("포인트");
                    sh.GetRow(3).GetCell(2).SetCellValue("0");
                }

                for (int i = 1; i <= 6; i++)
                {
                    using (var fs = new FileStream("database\\" + toyear + "\\"+i+"\\"+i+"반.xlsx", FileMode.Create, FileAccess.Write))
                    {
                        wb.Write(fs);
                    }

                }

                MessageBox.Show("정상적인 사용을 위하여 프로그램을 종료하신 후, 프로그램이 존재하는 폴더에서 database 폴더에 들어가신 후 올해 년도 폴더에 들어가시고 각 반 폴더 안에 존재하는 엑셀파일을 수정하여 주세요!");

                Environment.Exit(0);
            }

        }

       

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                System.Windows.Point canvPosToWindow = ((Image)sender).TransformToAncestor(this).Transform(new System.Windows.Point(0, 0));

                Image r = sender as Image;
                var upperlimit = canvPosToWindow.Y + (r.Height / 2);
                var lowerlimit = canvPosToWindow.Y + ((Image)sender).ActualHeight - (r.Height / 2);

                var leftlimit = canvPosToWindow.X + (r.Width / 2);
                var rightlimit = canvPosToWindow.X + ((Image)sender).ActualWidth - (r.Width / 2);


                var absmouseXpos = e.GetPosition(this).X;
                var absmouseYpos = e.GetPosition(this).Y;

                Console.WriteLine(absmouseXpos + " " + absmouseYpos);
                if ((absmouseXpos > leftlimit && absmouseXpos < rightlimit)
                    && (absmouseYpos > upperlimit && absmouseYpos < lowerlimit))
                {
                    r.SetValue(Canvas.LeftProperty, e.GetPosition(((Image)sender)).X - (r.Width / 2));
                    r.SetValue(Canvas.TopProperty, e.GetPosition(((Image)sender)).Y - (r.Height / 2));
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            Dragging = false;
        }

        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Count.Tag = "down";
            display.IsHitTestVisible = true;
            MenuOn.Begin();
            MenuOff.Stop();
        }


        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Count.Tag = "up";
            display.IsHitTestVisible = true;
            MenuOn.Begin();
            MenuOff.Stop();
            MouseDownLocation = e.GetPosition(this);
        }

        private void display_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectteam.SelectedItem = null;
            givecount.Text = null;
            display.IsHitTestVisible = false;
            MenuOff.Begin();
            MenuOn.Stop();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
            Dragging = true;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ScaleTransform scales = new ScaleTransform();
            scales.ScaleY = 1;
            scales.ScaleX = 1;
            String counttype = Count.Tag.ToString();
            TextBlock SelectTeam = null;
            int selectedban = selectban.SelectedIndex + 1;
            String selectteamnumber = "1";
            Image selectdragon = null;

            switch (((ComboBoxItem)selectteam.SelectedItem).Tag.ToString())
            {

                case "one":
                    SelectTeam = oneteam;
                    selectteamnumber = "1";
                    selectdragon = onedragon;
                    break;
                case "two":
                    SelectTeam = twoteam;
                    selectteamnumber = "2";
                    selectdragon = twodragon;
                    break;
                case "three":
                    SelectTeam = threeteam;
                    selectteamnumber = "3";
                    selectdragon = threedragon;
                    break;
                case "four":
                    SelectTeam = fourteam;
                    selectteamnumber = "4";
                    selectdragon = fourdragon;
                    break;
                case "five":
                    SelectTeam = fiveteam;
                    selectteamnumber = "5";
                    selectdragon = fivedragon;
                    break;
                case "six":
                    SelectTeam = sixteam;
                    selectteamnumber = "6";
                    selectdragon = sixdragon;
                    break;
                default:
                    break;
            }

            if(counttype == "up")
            {
                SelectTeam.Text = "X" + (Int32.Parse(SelectTeam.Text.Replace("X", "")) + Int32.Parse(givecount.Text)).ToString();

            } else if (counttype == "down") {
                SelectTeam.Text = "X" + (Int32.Parse(SelectTeam.Text.Replace("X", "")) - Int32.Parse(givecount.Text)).ToString();
            }

            scales.ScaleY = 1 + double.Parse(SelectTeam.Text.Replace("X", "")) * 0.02;
            scales.ScaleX = 1 + double.Parse(SelectTeam.Text.Replace("X", "")) * 0.02;
            selectdragon.RenderTransform = scales;

            using (var fs = new FileStream("database\\" + toyear + "\\" + selectedban + "\\" + selectedban + "반.xlsx", FileMode.Open, FileAccess.Read))
            {
                wb = new XSSFWorkbook(fs);
            }
            using (var fs = new FileStream("database\\" + toyear + "\\" + selectedban + "\\" + selectedban + "반.xlsx", FileMode.Create, FileAccess.Write))
            { 
                sh = (XSSFSheet)wb.GetSheet(selectteamnumber + "조");
                sh.GetRow(3).GetCell(2).SetCellValue(SelectTeam.Text.Replace("X", ""));
                wb.Write(fs);
            }
            display.IsHitTestVisible = false;
            MenuOff.Begin();
            MenuOn.Stop();
            var notificationSound = new SoundPlayer(中國語真棒.Properties.Resources.levelup);
            notificationSound.Play();
        }

        private void selectban_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

  

            int selectedban = ((ComboBox)sender).SelectedIndex + 1;
            using (var fs = new FileStream("database\\" + toyear + "\\" + selectedban + "\\" + selectedban + "반.xlsx", FileMode.Open, FileAccess.Read))
            {

                wb = new XSSFWorkbook(fs);

                sh = (XSSFSheet)wb.GetSheet("1조");
                oneteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selectone.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    oneteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    oneteam_member.Text =  " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString(); 
                }
                else
                {
                    oneteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    oneteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                onedragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
                oneteam.Text = "X" + sh.GetRow(3).GetCell(2).ToString();
                sh = (XSSFSheet)wb.GetSheet("2조");
                twoteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selecttwo.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    twoteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    twoteam_member.Text =  " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString();
                }
                else
                {
                    twoteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    twoteam_member.Text =  " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                twodragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
                twoteam.Text = "X" + sh.GetRow(3).GetCell(2).ToString();

                sh = (XSSFSheet)wb.GetSheet("3조");
                threeteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selectthree.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    threeteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    threeteam_member.Text =  " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString();
                }
                else
                {
                    threeteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    threeteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                threedragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
                threeteam.Text = "X" + sh.GetRow(3).GetCell(2).ToString();

                sh = (XSSFSheet)wb.GetSheet("4조");
                fourteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selectfour.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    fourteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    fourteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString();
                }
                else
                {
                    fourteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    fourteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                fourdragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
                fourteam.Text = "X" + sh.GetRow(3).GetCell(2).ToString();

                sh = (XSSFSheet)wb.GetSheet("5조");
                fiveteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selectfive.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    fiveteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    fiveteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString();
                }
                else
                {
                    fiveteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    fiveteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                fivedragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
                fiveteam.Text = "X" +  sh.GetRow(3).GetCell(2).ToString();

                sh = (XSSFSheet)wb.GetSheet("6조");
                sixteam_name.Text = sh.GetRow(1).GetCell(2).ToString();
                selectsix.Content = sh.GetRow(1).GetCell(2).ToString();
                if (sh.GetRow(2).GetCell(8) == null)
                {
                    sixteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    sixteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString();
                }
                else
                {
                    sixteam_boss.Text = sh.GetRow(2).GetCell(2).ToString();
                    sixteam_member.Text = " " + sh.GetRow(2).GetCell(3).ToString() + " " + sh.GetRow(2).GetCell(4).ToString() + " " + sh.GetRow(2).GetCell(5).ToString() + " " + sh.GetRow(2).GetCell(6).ToString() + " " + sh.GetRow(2).GetCell(7).ToString() + " " + sh.GetRow(2).GetCell(8).ToString(); ;
                }
                sixteam.Text = "X" + sh.GetRow(3).GetCell(2).ToString();
                sixdragon.RenderTransform = new ScaleTransform(1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02, 1 + double.Parse(sh.GetRow(3).GetCell(2).ToString()) * 0.02);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F2)
            {
                NewsViewer newsviewer = new NewsViewer();
                newsviewer.Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        //private void Image_MouseMove_1(object sender, MouseEventArgs e)
        //{
        //    if(e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Thickness margin = dragon.Margin;
        //        margin.Left = e.GetPosition(this).X + dragon.Margin.Left - MouseDownLocation.X;
        //        margin.Top = e.GetPosition(this).Y + dragon.Margin.Top - MouseDownLocation.Y;
        //        dragon.Margin = margin;
        //    }
        //}
    }
}
