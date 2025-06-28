using System.Collections.ObjectModel;
using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class TransactionsView : ContentView
{
    private readonly ITransactionService _transactionService;
    
    public ObservableCollection<Transaction> Transactions { get; set; } = new();

    // Events for transaction modifications
    public event EventHandler? TransactionModified;

    public TransactionsView()
    {
        InitializeComponent();
        _transactionService = new ApiTransactionService(); 
        BindingContext = this;
        LoadTransactions();
    }

    public void OnTabChanged(object sender, string section)
    {
        // Refresh data when transactions tab becomes active
        if (section == "transactions")
        {
            LoadTransactions();
        }
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
        // Reset the form to create mode and show it
        AddTransactionView.ResetToCreateMode();
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
        
        // Notify that a transaction was modified
        TransactionModified?.Invoke(this, EventArgs.Empty);
    }

    private void OnTransactionUpdated(object sender, EventArgs e)
    {
        // Hide the Add Transaction view and show the transactions list
        AddTransactionView.IsVisible = false;
        TransactionsListGrid.IsVisible = true;
        
        // Refresh the transactions list
        LoadTransactions();
        
        // Notify that a transaction was modified
        TransactionModified?.Invoke(this, EventArgs.Empty);
    }

    private void OnTransactionCancelled(object sender, EventArgs e)
    {
        // Hide the Add Transaction view and show the transactions list
        AddTransactionView.IsVisible = false;
        TransactionsListGrid.IsVisible = true;
    }

    private async void OnEditTransactionClicked(object sender, Transaction transaction)
    {
        // Show the Add Transaction view in edit mode
        TransactionsListGrid.IsVisible = false;
        AddTransactionView.IsVisible = true;
        
        // Set the view to edit mode
        await AddTransactionView.SetEditMode(transaction.Id);
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
                
                // Notify that a transaction was modified
                TransactionModified?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Error", "Failed to delete transaction: " + ex.Message, "OK");
            }
        }
    }
} 