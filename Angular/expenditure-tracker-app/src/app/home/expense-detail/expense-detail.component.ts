import { ChangeDetectionStrategy, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDatepicker } from '@angular/material/datepicker';
import { AddTransactionExpenseFormComponent } from '../forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { ExpenseDialogData, Transactions, TransactionTableFilter } from '../../shared/interfaces/interfaces';
import { SharedService } from '../../shared/services/shared.service';
import { TransactionService } from '../../shared/services/transaction.service';

import * as _moment from 'moment';
import { default as _rollupMoment, Moment } from 'moment';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { FormControl } from '@angular/forms';

const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
  parse: {
    dateInput: 'MM/YYYY',
  },
  display: {
    dateInput: 'MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-expense-detail',
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.css',
  providers: [provideMomentDateAdapter(MY_FORMATS)],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,

})

export class ExpenseDetailComponent implements OnInit {
  constructor(private router: Router,
    private dialog: MatDialog,
    private sharedService: SharedService,
    private transactionExpenseService: TransactionService) { }

  displayedColumns: string[] = ['id', 'date', 'category', 'transactionType', 'amount', 'note', 'edit', 'delete'];

  //Filter fields.
  month: number;
  year: number; 

  //Values used for the transaction table.
  transactions: Transactions[] = [];
  transactionTableData = new MatTableDataSource(this.transactions);

  transactionRow: Transactions = this.initializeTransactions();

  loggedInUserId: number; //Fetch login user data to be used for identification in backend APIs.

  editable = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  selectedMonthAndYear: Moment = moment(); // Initialize with the current date
  filter: TransactionTableFilter;

  ngOnInit(): void {
    this.sharedService.userIdSubject.subscribe((data) => {
      this.loggedInUserId = data;
      this.fetchDataForMonth(this.selectedMonthAndYear); // Fetch data for the current month on init
    });
  }

  initializeTransactions(): Transactions {
    return {
      id: 0,
      transactionDate: new Date(),
      user_Id: 0,
      category_Id: 0,
      category_Name: '',
      transactionType_Id: 0,
      transactionType_Name: '',
      amount: 0,
      note: ''
    }; //Fields to be passed to the dialog form. Pass a defualt value at times.
  }

  initializeFilters(): TransactionTableFilter {
    return {
      user_Id: this.loggedInUserId,
      month: this.month,
      year: this.year,
    }
  }

  addTransactionExpense() {
    const dialogData: ExpenseDialogData = {
      transactionRow: this.transactionRow,
      loggedInUserId: this.loggedInUserId,
      editable: this.editable
    };
    const dialogRef = this.dialog.open(AddTransactionExpenseFormComponent, {
      width: '400px',  // Optional: set width of the dialog
      data: dialogData         // Optional: pass data to dialog if needed
    });

    // Handle dialog close (optional)
    dialogRef.afterClosed().subscribe(result => {
      // Logic after the dialog is closed, if needed
      this.transactionRow.id = 0; //reset value.
      this.transactionRow = this.initializeTransactions();
      this.fetchDataForMonth(this.selectedMonthAndYear);
    });
  }

  editTransaction(rowData: Transactions) {
    this.transactionRow = rowData;
    this.editable = true;
    this.addTransactionExpense();
  }

  removeTransaction(transactionId: number) {

    this.transactionExpenseService.deleteTransaction(transactionId).subscribe(() => {
      this.fetchDataForMonth(this.selectedMonthAndYear);
    });
  }

  setMonthAndYear(normalizedMonthAndYear: Moment, datepicker: MatDatepicker<Moment>) {
    this.selectedMonthAndYear = normalizedMonthAndYear; // Update the selected date
    datepicker.close();

    // Fetch data for the selected month
    this.fetchDataForMonth(this.selectedMonthAndYear);
  }

  // Fetch data from the backend for the selected month
  fetchDataForMonth(selectedDate: Moment) {
    this.month = selectedDate.month() + 1; // Months are 0-based in Moment.js
    this.year = selectedDate.year();

    this.filter = this.initializeFilters();

    this.transactionExpenseService.getTransactionsByFilter(this.filter).subscribe((response) => {
      this.transactionTableData.data = response;
      this.transactionTableData.paginator = this.paginator;
    });

  }
}
