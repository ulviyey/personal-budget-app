﻿using Cardify.MAUI.Pages;

namespace Cardify.MAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
	}
}
