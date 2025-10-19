#include <iostream>
using namespace std;

int main() {
    float buyPrice, sellPrice;
    int quantity;
    float profitOrLoss;
    int choice;

    cout << "=== Simple Stock Exchange Menu ===" << endl;
    cout << "1. Calculate Profit or Loss" << endl;
    cout << "2. Exit" << endl;
    cout << "Enter your choice: ";
    cin >> choice;

    if (choice == 1) {
        cout << "\nEnter buying price per share: ";
        cin >> buyPrice;

        cout << "Enter selling price per share: ";
        cin >> sellPrice;

        cout << "Enter number of shares: ";
        cin >> quantity;

        profitOrLoss = (sellPrice - buyPrice) * quantity;

        if (profitOrLoss > 0)
            cout << ":-) You made a profit of $" << profitOrLoss << endl;
        else if (profitOrLoss < 0)
            cout << ":-( You incurred a loss of $" << -profitOrLoss << endl;
        else
            cout << ":-| No profit, no loss." << endl;
    }
    else if (choice == 2) {
        cout << "Exiting program... Goodbye!" << endl;
    }
    else {
        cout << "Invalid choice! Please run the program again." << endl;
    }

    return 0;
}
