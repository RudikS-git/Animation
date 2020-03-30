using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace Laba4
{
    public partial class MainWindow : Window
    {
        DispatcherTimer frameTimer;

        Image imageFirst;

        BitmapImage bitmapGirl;
        BitmapImage bitmapBoy;

        List<CroppedBitmap> cordGirlForward;
        List<CroppedBitmap> cordGirlBack;

        List<CroppedBitmap> cordBoyForward;
        List<CroppedBitmap> cordBoyBack;

        Storyboard pathAnimationStoryboard;
        DoubleAnimationUsingPath translateXAnimation;
        PathGeometry pathRight;
        PathGeometry pathLeft;

        int currentFrame = 0;
        bool FlagMode = true;
        bool FlagTypeMovement = true;

        public MainWindow()
        {
            InitializeComponent();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin"));
            InitSprites(path);

            frameTimer = new DispatcherTimer();
            frameTimer.Interval = TimeSpan.FromSeconds(0.2);
            frameTimer.Tick += OnFrame;

            TranslateTransform animatedTranslateTransform = new TranslateTransform(); // Перемещение
            canvas.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform); // Регистрируем чтобы начать раскадровку
            imageFirst.RenderTransform = animatedTranslateTransform;

            // Создаём пути для анимации
            pathRight = CreatePathForAnimation(50, 230, 900, 230);
            pathLeft = CreatePathForAnimation(900, 230, 50, 230);

            pathAnimationStoryboard = new Storyboard();
            CreateAnimation(pathRight);
        }

        private void InitSprites(string path)
        {
            bitmapGirl = new BitmapImage();
            bitmapGirl.BeginInit();
            bitmapGirl.UriSource = new Uri(path + @"\rungirl\1_3.png", UriKind.Relative);
            bitmapGirl.CacheOption = BitmapCacheOption.OnLoad;
            bitmapGirl.EndInit();

            bitmapBoy = new BitmapImage();
            bitmapBoy.BeginInit();
            bitmapBoy.UriSource = new Uri(path + @"\runboy\1_1.png", UriKind.Relative);
            bitmapBoy.CacheOption = BitmapCacheOption.OnLoad;
            bitmapBoy.EndInit();

            cordGirlForward = new List<CroppedBitmap>()
            {
                new CroppedBitmap(bitmapGirl, new Int32Rect(0, 124, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(125, 124, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(249, 124, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(370, 124, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(500, 124, 115, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(620, 124, 115, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(740, 124, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(860, 124, 121, 118))
            };

            cordGirlBack = new List<CroppedBitmap>()
            {
                new CroppedBitmap(bitmapGirl, new Int32Rect(0, 0, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(115, 0, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(239, 0, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(360, 0, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(490, 0, 115, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(610, 0, 115, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(735, 0, 121, 118)),
                new CroppedBitmap(bitmapGirl, new Int32Rect(850, 0, 121, 118))
            };

            cordBoyForward = new List<CroppedBitmap>()
            {
                new CroppedBitmap(bitmapBoy, new Int32Rect(0, 0, 131, 150)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(145, 0, 131, 150)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(290, 0, 131, 150)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(445, 0, 131, 150)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(620, 0, 135, 150)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(750, 0, 135, 150)),
            };

            cordBoyBack = new List<CroppedBitmap>()
            {
                new CroppedBitmap(bitmapBoy, new Int32Rect(0, 170, 131, 170)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(145, 170, 131, 170)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(290, 170, 131, 170)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(445, 170, 131, 170)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(620, 170, 130, 170)),
                new CroppedBitmap(bitmapBoy, new Int32Rect(750, 170, 135, 170)),
            };

            imageFirst = new Image();
            imageFirst.Source = cordGirlForward[0];
            imageFirst.Width = 100;
            imageFirst.Height = 100;
            imageFirst.Margin = new Thickness(-100, 230, 0, 0);
            canvas.Children.Add(imageFirst);
        }

        private PathGeometry CreatePathForAnimation(int x1, int y1, int x2, int y2)
        {
            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.IsClosed = false;
            pFigure.StartPoint = new Point(x1, y1);
            PolyLineSegment pLineSegment = new PolyLineSegment();
            pLineSegment.Points.Add(new Point(x2, y2));
            pFigure.Segments.Add(pLineSegment);
            animationPath.Figures.Add(pFigure);
            animationPath.Freeze(); // Замораживаем для производительности

            return animationPath;
        }

        private void CreateAnimation(PathGeometry path)
        {
            translateXAnimation = new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = path;
            translateXAnimation.Duration = TimeSpan.FromSeconds(8);
            translateXAnimation.Source = PathAnimationSource.X;
            Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform"); // Устанавливаем анимацию
            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath(TranslateTransform.XProperty));

            pathAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            pathAnimationStoryboard.Children.Add(translateXAnimation); // размещаем анимацию
        }

        private void OnFrame(object sender, EventArgs e)
        {
            if (FlagMode)
            {
                if(currentFrame == cordGirlForward.Count)
                {
                    currentFrame = 0;
                }

                if (FlagTypeMovement)
                {
                    imageFirst.Source = cordGirlForward[currentFrame];
                }
                else
                {
                    imageFirst.Source = cordGirlBack[currentFrame];
                }
            }
            else
            {
                if (currentFrame == cordBoyForward.Count)
                {
                    currentFrame = 0;
                }

                if (FlagTypeMovement)
                {
                    imageFirst.Source = cordBoyForward[currentFrame];
                }
                else
                {
                    imageFirst.Source = cordBoyBack[currentFrame];
                }
            }

            currentFrame++;
            label.Content = $"Номер спрайта: {currentFrame.ToString()}";
        }

        private void TypeObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;

            if (cb.SelectedIndex == 0)
            {
                FlagMode = true;
                currentFrame = 0;
            }
            else
            {
                FlagMode = false;
                currentFrame = 0;
            }

        }

        private void TypeMovement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;

            if (cb.SelectedIndex == 0)
            {
                FlagTypeMovement = true;
                if (translateXAnimation != null)
                {
                    translateXAnimation.PathGeometry = pathRight;
                }
            }
            else
            {
                FlagTypeMovement = false;

                if (translateXAnimation != null)
                {
                    translateXAnimation.PathGeometry = pathLeft;
                }
            }
                
            if(pathAnimationStoryboard != null)
            {
                pathAnimationStoryboard.Pause(canvas);
                pathAnimationStoryboard.Begin(canvas, true);
                Button1.Content = "Остановить";
                frameTimer.Start();
            }
        }

        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            if ((String)Button1.Content == "Начать")
            {
                Button1.Content = "Остановить";
                frameTimer.Start();
                pathAnimationStoryboard.Begin(canvas, true);
            }
            else
            {
                Button1.Content = "Начать";
                frameTimer.Stop();
                pathAnimationStoryboard.Pause(canvas);
                
            }
        }
    }
}