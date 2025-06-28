using Cardify.MAUI.Models;

namespace Cardify.MAUI.Views;

public partial class CardItemView : ContentView
{
    public event EventHandler<Card>? EditCardClicked;
    public event EventHandler<Card>? DeleteCardClicked;

    public CardItemView()
    {
        InitializeComponent();
    }

    private void OnEditCardClicked(object sender, EventArgs e)
    {
        if (BindingContext is Card card)
        {
            EditCardClicked?.Invoke(this, card);
        }
    }

    private void OnDeleteCardClicked(object sender, EventArgs e)
    {
        if (BindingContext is Card card)
        {
            DeleteCardClicked?.Invoke(this, card);
        }
    }
} 