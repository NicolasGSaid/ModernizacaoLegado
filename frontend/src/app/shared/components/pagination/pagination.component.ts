import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, MatPaginatorModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css'
})
export class PaginationComponent {
  // Inputs compatíveis com PaginatedResultDto do backend
  page = input<number>(1);
  pageSize = input<number>(10);
  totalItems = input<number>(0);
  pageSizeOptions = input<number[]>([5, 10, 25, 50]);

  // Outputs
  pageChange = output<number>();
  pageSizeChange = output<number>();

  onPageChange(event: PageEvent): void {
    // Material Paginator usa índice baseado em 0, backend usa baseado em 1
    this.pageChange.emit(event.pageIndex + 1);
    this.pageSizeChange.emit(event.pageSize);
  }
}
