import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExpenseDashboardComponent } from './expense-dashboard/expense-dashboard.component';
import { ExpenseDetailComponent } from './expense-detail/expense-detail.component';
import { ExpenseLayoutComponent } from './expense-layout/expense-layout.component';
import { CategoryDetailComponent } from './category-detail/category-detail.component';

const routes: Routes = [
  {
    path: '', component: ExpenseLayoutComponent,
    children: [
      { path: 'dashboard', component: ExpenseDashboardComponent },
      { path: 'transactions-detail', component: ExpenseDetailComponent },
      { path: 'categories-detail', component: CategoryDetailComponent },
      {
        path: '**',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
