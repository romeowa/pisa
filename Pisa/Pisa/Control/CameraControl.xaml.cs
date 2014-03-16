using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Pisa.Control
{
	public partial class CameraControl : UserControl
	{
		public CameraControl()
		{
			InitializeComponent();
			this.Loaded += CameraControl_Loaded;
		}

		void CameraControl_Loaded(object sender, RoutedEventArgs e)
		{
			PhotoCamera camera = new PhotoCamera(CameraType.Primary);
			VideoViewfinderBrush.SetSource(camera);
			VideoViewfinderTransform.Rotation = camera.Orientation;
		}
	}
}
