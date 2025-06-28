using Cardify.MAUI.Models;

namespace Cardify.MAUI.Views;

public partial class TransactionCardTemplate : ContentView
{
    public event EventHandler<Transaction>? EditClicked;
    public event EventHandler<Transaction>? DeleteClicked;

    public TransactionCardTemplate()
    {
        InitializeComponent();
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        if (BindingContext is Transaction transaction)
        {
            EditClicked?.Invoke(this, transaction);
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (BindingContext is Transaction transaction)
        {
            DeleteClicked?.Invoke(this, transaction);
        }
    }
} 