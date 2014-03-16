using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Pisa.Control
{
	public partial class CameraControl : UserControl
	{
		PhotoCamera _camera;
		public CameraControl()
		{
			InitializeComponent();

			VisualStateManager.GoToState(this, BeforeCaptureState.Name, false);

			LinkEvents();
			InitCamera();
		}

		private void LinkEvents()
		{
			ViewportRectangle.Tap += ViewportRectangle_Tap;
			CapturedImage.Tap += CapturedImage_Tap;
		}

		void CapturedImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			VisualStateManager.GoToState(this, StartCaptureState.Name, false);
		}

		async void ViewportRectangle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			WriteableBitmap bitmap = new WriteableBitmap(ViewportRectangle, null);
			string fileName = Guid.NewGuid().ToString();

			using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
			{
				using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
				{
					int width = (int)ViewportRectangle.ActualWidth;
					int height = (int)ViewportRectangle.ActualHeight;
					await Task.Run(() =>
					{
						bitmap.SaveJpeg(isfStream, width, height, 0, 100);
					});
				}

				using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
				{
					BitmapImage bitmapImage = new BitmapImage();
					bitmapImage.SetSource(isfStream);
					CapturedImage.Source = bitmapImage;
					VisualStateManager.GoToState(this, CompletedCaptureState.Name, false);
				}
			}
		}

		private void InitCamera()
		{
			_camera = new PhotoCamera(CameraType.Primary);
			_camera.Initialized += _camera_Initialized;

			VideoViewfinderBrush.SetSource(_camera);
			VideoViewfinderTransform.Rotation = _camera.Orientation;
		}

		void _camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
		{
			if (e.Succeeded == true)
			{
				Dispatcher.BeginInvoke(() =>
				{
					VisualStateManager.GoToState(this, StartCaptureState.Name, false);
				});
			}
			else
			{
				//TODO : 카메라 초기화 실패시에 재시도 처리가 필요함.
			}
		}
	}
}
