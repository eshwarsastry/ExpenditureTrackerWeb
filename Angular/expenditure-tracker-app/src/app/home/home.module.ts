import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { ExpenseDetailComponent } from './expense-detail/expense-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../shared/material/material.module';
import { AddTransactionExpenseFormComponent } from './forms/add-transaction-expense-form/add-transaction-expense-form.component';
import { AddTransactionCategoryFormComponent } from './forms/add-transaction-category-form/add-transaction-category-form.component';
import { ExpenseLayoutComponent } from './expense-layout/expense-layout.component';
import { CategoryDetailComponent } from './category-detail/category-detail.component';
import { ExpenseDashboardComponent } from './expense-dashboard/expense-dashboard.component';
import { ImportCsvPopupComponent } from './forms/import-csv-popup/import-csv-popup.component';
import { ExpenseAnalysisComponent } from './expense-analysis/expense-analysis.component';
import { provideNativeDateAdapter } from '@angular/material/core';


@NgModule({
  declarations: [
    ExpenseDetailComponent,
    ExpenseDashboardComponent,
    AddTransactionExpenseFormComponent,
    AddTransactionCategoryFormComponent,
    ExpenseLayoutComponent,
    CategoryDetailComponent,
    ImportCsvPopupComponent,
    ExpenseAnalysisComponent
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    provideNativeDateAdapter()
  ]
})
export class HomeModule { }
