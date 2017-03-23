using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MySuperGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Butterfly
        private Butterfly butterfly;

        // Flowers, flower
        private List<Flower> flowers;

        // Which keys are pressed
        private bool UpPressed;
        private bool LeftPressed;
        private bool RightPressed;

        // Game loop timer
        private DispatcherTimer timer;

        // Audio
        private MediaElement mediaElement;


        public MainPage()
        {
            this.InitializeComponent();

            // Create a butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2,
                LocationY = MyCanvas.Height / 2
            };
            // Add butterfly to Canvas
            MyCanvas.Children.Add(butterfly);

            // Init list of flowers
            flowers = new List<Flower>();

            // Key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // Mouse listener
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            // Load audio
            LoadAudio();

            // Start game loop
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Load audio use when colliding
        private async void LoadAudio()
        {
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file = await folder.GetFileAsync("ding.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            mediaElement.SetSource(stream, file.ContentType);
        }


        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;

            // Add to canvas
            MyCanvas.Children.Add(flower);
            flower.SetLocation();

            // Add to flower list
            flowers.Add(flower);
        }

        // Game loop
        private void Timer_Tick(object sender, object e)
        {
            // Move butterfly if up pressed
            if (UpPressed) butterfly.Move();

            // Rotate butterfly is left/right pressed
            // -1 == left, 1 == right
            if (LeftPressed) butterfly.Rotate(-1); 
            if (RightPressed) butterfly.Rotate(1);

            // Update butterfly
            butterfly.SetLocation();

            // Collision...
            CheckCollision();
        }

        private void CheckCollision()
        {
            // Loop flowers list
            foreach(Flower flower in flowers)
            {
                // Get rects
                Rect BRect = new Rect(butterfly.LocationX, butterfly.LocationY, butterfly.ActualWidth, butterfly.ActualHeight);
                Rect FRect = new Rect(flower.LocationX, flower.LocationY, flower.ActualWidth, flower.ActualHeight);

                // Does objects intersects
                BRect.Intersect(FRect);
                if (!BRect.IsEmpty)
                {
                    // Collision! area isn't empty
                    // Remove flower from Canvas
                    MyCanvas.Children.Remove(flower);
                    // Remove from list
                    flowers.Remove(flower);

                    //play audio
                    mediaElement.Play();
                    break;
                }
            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;
            }
        }
    }
}
