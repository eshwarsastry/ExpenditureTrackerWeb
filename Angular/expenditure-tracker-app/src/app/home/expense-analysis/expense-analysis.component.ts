import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import * as d3 from 'd3';
import { TransactionService } from '../../shared/services/transaction.service';
import { SharedService } from '../../shared/services/shared.service';
import { Transactions } from '../../shared/interfaces/interfaces';
import { NumberValue } from 'd3';

@Component({
  selector: 'app-expense-analysis',
  templateUrl: './expense-analysis.component.html',
  styleUrls: ['./expense-analysis.component.css']
})
export class ExpenseAnalysisComponent implements OnInit, AfterViewInit {
  dateRange: FormGroup;
  userId: number = 0;
  allTransactions: Transactions[] = [];

  constructor(
    private fb: FormBuilder,
    private transactionService: TransactionService,
    private sharedService: SharedService
  ) {}

  ngOnInit(): void {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    this.dateRange = this.fb.group({
      start: [startOfMonth],
      end: [now]
    });
    this.sharedService.userIdSubject.subscribe(userId => {
      this.userId = userId;
      this.fetchTransactions();
    });
    this.dateRange.valueChanges.subscribe(() => {
      this.renderChart();
    });
  }

  ngAfterViewInit(): void {
    this.renderChart();
  }

  fetchTransactions(): void {
    if (!this.userId) return;
    this.transactionService.getAllTransactions(this.userId).subscribe(transactions => {
      this.allTransactions = transactions;
      this.renderChart();
    });
  }

  renderChart(): void {
    // Remove previous charts
    d3.select('#d3-chart').selectAll('*').remove();
    d3.select('#d3-pie-chart').selectAll('*').remove();
    if (!this.allTransactions.length) return;
    const start: Date = this.dateRange.value.start;
    const end: Date = this.dateRange.value.end;
    // Prepare date keys
    const dateKeys: string[] = [];
    const current = new Date(start);
    while (current <= end) {
      const key = `${current.getFullYear()}-${(current.getMonth()+1).toString().padStart(2, '0')}-${current.getDate().toString().padStart(2, '0')}`;
      dateKeys.push(key);
      current.setDate(current.getDate() + 1);
    }
    // Aggregate income and expenditure per day
    const incomeMap: { [date: string]: number } = {};
    const expenseMap: { [date: string]: number } = {};
    for (const key of dateKeys) {
      incomeMap[key] = 0;
      expenseMap[key] = 0;
    }
    this.allTransactions.forEach(tx => {
      const txDateObj = new Date(tx.transactionDate);
      const txDate = `${txDateObj.getFullYear()}-${(txDateObj.getMonth()+1).toString().padStart(2, '0')}-${txDateObj.getDate().toString().padStart(2, '0')}`;
      if (txDate >= dateKeys[0] && txDate <= dateKeys[dateKeys.length - 1]) {
        if (tx.transactionType_Name.toLowerCase() === 'income') {
          incomeMap[txDate] += tx.amount;
        } else if (tx.transactionType_Name.toLowerCase() === 'expenditure') {
          expenseMap[txDate] += Math.abs(tx.amount);
        }
      }
    });

    const incomeData = dateKeys.map(date => ({ date, value: incomeMap[date] }));
    const expenseData = dateKeys.map(date => ({ date, value: expenseMap[date] }));
    // Calculate cumulative sums for income and expense
    let cumulativeIncome = 0;
    let cumulativeExpense = 0;
    const incomeCumulativeData = dateKeys.map(date => {
      cumulativeIncome += incomeMap[date];
      return { date, value: cumulativeIncome };
    });
    const expenseCumulativeData = dateKeys.map(date => {
      cumulativeExpense += expenseMap[date];
      return { date, value: cumulativeExpense };
    });
    // D3 line chart (with points and tooltips)
    const width = 700;
    const height = 350;
    const margin = { top: 30, right: 40, bottom: 40, left: 60 };
    const svg = d3.select('#d3-chart')
      .append('svg')
      .attr('width', width)
      .attr('height', height);
    const x = d3.scaleTime()
      .domain([new Date(dateKeys[0]), new Date(dateKeys[dateKeys.length - 1])])
      .range([margin.left, width - margin.right]);
    const y = d3.scaleLinear()
      .domain([0, d3.max([...incomeCumulativeData, ...expenseCumulativeData], d => d.value) || 1])
      .nice()
      .range([height - margin.bottom, margin.top]);
    // X axis
    svg.append('g')
      .attr('transform', `translate(0,${height - margin.bottom})`)
      .call(d3.axisBottom(x).ticks(8).tickFormat((domainValue: Date | NumberValue, _i: number) => {
        const date = domainValue instanceof Date ? domainValue : new Date(domainValue as number);
        return d3.timeFormat('%b %d')(date);
      }));
    // Y axis
    svg.append('g')
      .attr('transform', `translate(${margin.left},0)`)
      .call(d3.axisLeft(y));
    // Line generator
    const line = d3.line<{ date: string; value: number }>()
      .x(d => x(new Date(d.date)))
      .y(d => y(d.value));
    // Draw cumulative income line
    svg.append('path')
      .datum(incomeCumulativeData)
      .attr('fill', 'none')
      .attr('stroke', '#43a047')
      .attr('stroke-width', 2)
      .attr('d', line);
    // Draw cumulative expense line
    svg.append('path')
      .datum(expenseCumulativeData)
      .attr('fill', 'none')
      .attr('stroke', '#e53935')
      .attr('stroke-width', 2)
      .attr('d', line);
    // Tooltip
    const tooltip = d3.select('body').append('div')
      .attr('class', 'd3-tooltip')
      .style('position', 'absolute')
      .style('background', '#fff')
      .style('border', '1px solid #888')
      .style('padding', '8px 12px')
      .style('border-radius', '6px')
      .style('pointer-events', 'none')
      .style('font-size', '13px')
      .style('box-shadow', '0 2px 8px rgba(0,0,0,0.08)')
      .style('display', 'none')
      .style('z-index', '9999');
    // Draw points for each transaction (income and expenditure)
    this.allTransactions.forEach(tx => {
      const txDateObj = new Date(tx.transactionDate);
      const txDate = `${txDateObj.getFullYear()}-${(txDateObj.getMonth()+1).toString().padStart(2, '0')}-${txDateObj.getDate().toString().padStart(2, '0')}`;
      if (tx.transactionType_Name.toLowerCase() === 'income' && txDate >= dateKeys[0] && txDate <= dateKeys[dateKeys.length - 1]) {
        svg.append('circle')
          .attr('cx', x(new Date(txDate)))
          .attr('cy', y(incomeCumulativeData.find(d => d.date === txDate)?.value ?? 0))
          .attr('r', 6)
          .attr('fill', '#43a047')
          .on('mouseover', function (event) {
            tooltip.style('display', 'block')
              .html(`<b>Date:</b> ${txDate}<br><b>Amount:</b> ${tx.amount}<br><b>Category:</b> ${tx.category_Name}`)
              .style('left', (event.pageX + 20) + 'px')
              .style('top', (event.pageY - 20) + 'px');
          })
          .on('mousemove', function (event) {
            tooltip.style('left', (event.pageX + 20) + 'px')
              .style('top', (event.pageY - 20) + 'px');
          })
          .on('mouseout', function () {
            tooltip.style('display', 'none');
          });
      }
      if (tx.transactionType_Name.toLowerCase() === 'expenditure' && txDate >= dateKeys[0] && txDate <= dateKeys[dateKeys.length - 1]) {
        svg.append('circle')
          .attr('cx', x(new Date(txDate)))
          .attr('cy', y(expenseCumulativeData.find(d => d.date === txDate)?.value ?? 0))
          .attr('r', 6)
          .attr('fill', '#e53935')
          .on('mouseover', function (event) {
            tooltip.style('display', 'block')
              .html(`<b>Date:</b> ${txDate}<br><b>Amount:</b> ${tx.amount}<br><b>Category:</b> ${tx.category_Name}`)
              .style('left', (event.pageX + 20) + 'px')
              .style('top', (event.pageY - 20) + 'px');
          })
          .on('mousemove', function (event) {
            tooltip.style('left', (event.pageX + 20) + 'px')
              .style('top', (event.pageY - 20) + 'px');
          })
          .on('mouseout', function () {
            tooltip.style('display', 'none');
          });
      }
    });
    // Add legend
    svg.append('circle').attr('cx', width - 120).attr('cy', 40).attr('r', 8).style('fill', '#43a047');
    svg.append('text').attr('x', width - 105).attr('y', 44).text('Income').style('font-size', '15px').attr('alignment-baseline', 'middle');
    svg.append('circle').attr('cx', width - 120).attr('cy', 70).attr('r', 8).style('fill', '#e53935');
    svg.append('text').attr('x', width - 105).attr('y', 74).text('Expense').style('font-size', '15px').attr('alignment-baseline', 'middle');
    // Pie chart for expenses by category
    this.renderPieChart(start, end);
  }

  renderPieChart(start: Date, end: Date): void {
    // Filter only expenses in date range
    const filtered = this.allTransactions.filter(tx => {
      const txDateObj = new Date(tx.transactionDate);
      const txDate = `${txDateObj.getFullYear()}-${(txDateObj.getMonth()+1).toString().padStart(2, '0')}-${txDateObj.getDate().toString().padStart(2, '0')}`;
      const startKey = `${start.getFullYear()}-${(start.getMonth()+1).toString().padStart(2, '0')}-${start.getDate().toString().padStart(2, '0')}`;
      const endKey = `${end.getFullYear()}-${(end.getMonth()+1).toString().padStart(2, '0')}-${end.getDate().toString().padStart(2, '0')}`;
      return (
        tx.transactionType_Name.toLowerCase() === 'expenditure' &&
        txDate >= startKey && txDate <= endKey
      );
    });
    // Group by category and sum
    const categorySums: { [category: string]: number } = {};
    filtered.forEach(tx => {
      if (!categorySums[tx.category_Name]) categorySums[tx.category_Name] = 0;
      categorySums[tx.category_Name] += Math.abs(tx.amount);
    });
    const data = Object.entries(categorySums).map(([category, value]) => ({ category, value: Number(value) }));
    console.log('Pie chart data:', data);
    if (!data.length) {
      d3.select('#d3-pie-chart').append('div').style('color', 'red').style('text-align', 'center').style('margin-top', '40px').text('No expense data for this date range.');
      return;
    }
    const width = 350, height = 250, radius = Math.min(width, height) / 2;
    const svg = d3.select('#d3-pie-chart')
      .append('svg')
      .attr('width', width)
      .attr('height', height)
      .style('background', '#fffbe7')
      .append('g')
      .attr('transform', `translate(${width / 2},${height / 2})`);
    console.log('Pie chart SVG:', svg);
    const color = d3.scaleOrdinal(d3.schemeCategory10);
    const pie = d3.pie<{ category: string; value: number }>().value(d => d.value);
    const arc = d3.arc<d3.PieArcDatum<{ category: string; value: number }>>()
      .innerRadius(0)
      .outerRadius(radius - 10);
    const arcs = svg.selectAll('arc')
      .data(pie(data))
      .enter()
      .append('g');
    arcs.append('path')
      .attr('d', arc)
      .attr('fill', d => color(d.data.category));
    arcs.append('text')
      .attr('transform', d => `translate(${arc.centroid(d)})`)
      .attr('dy', '0.35em')
      .attr('text-anchor', 'middle')
      .style('font-size', '12px')
      .text(d => d.data.category);
  }
} 