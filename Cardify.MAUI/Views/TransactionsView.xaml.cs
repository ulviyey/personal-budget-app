using System.Collections.ObjectModel;
using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class TransactionsView : ContentView
{
    private readonly ITransactionService _transactionService;
    
    public ObservableCollection<Transaction> Transactions { get; set; } = new();

    public TransactionsView()
    {
        InitializeComponent();
        _transactionService = new ApiTransactionService(); 
        BindingContext = this;
        LoadTransactions();
    }

    private async void LoadTransactions()
    {
        try
        {
            var transactions = await _transactionService.GetTransactionsAsync();
            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to load transactions: " + ex.Message, "OK");
        }
    }

    private void OnAddTransactionClicked(object sender, EventArgs e)
    {
        // Show the Add Transaction view
        TransactionsListGrid.IsVisible = false;
        AddTransactionView.IsVisible = true;
    }

    private void OnTransactionAdded(object sender, EventArgs e)
    {
        // Hide the Add Transaction view and show the transactions list
        AddTransactionView.IsVisible = false;
        TransactionsListGrid.IsVisible = true;
        
        // Refresh the transactions list
        LoadTransactions();
    }

    private void OnTransactionCancelled(object sender, EventArgs e)
    {
        // Hide the Add Transaction view and show the transactions list
        AddTransactionView.IsVisible = false;
        TransactionsListGrid.IsVisible = true;
    }

    private async void OnEditTransactionClicked(object sender, Transaction transaction)
    {
        // TODO: Implement edit transaction functionality
        await Application.Current.Windows[0].Page.DisplayAlert("Coming Soon", "Edit transaction functionality will be implemented next!", "OK");
    }

    private async void OnDeleteTransactionClicked(object sender, Transaction transaction)
    {
        var result = await Application.Current.Windows[0].Page.DisplayAlert(
            "Delete Transaction", 
            $"Are you sure you want to delete '{transaction.Name}'?", 
            "Delete", "Cancel");

        if (result)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(transaction.Id);
                Transactions.Remove(transaction); // Remove from the collection
                await Application.Current.Windows[0].Page.DisplayAlert("Success", "Transaction deleted successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to delete transaction: " + ex.Message, "OK");
            }
        }
    }
} 