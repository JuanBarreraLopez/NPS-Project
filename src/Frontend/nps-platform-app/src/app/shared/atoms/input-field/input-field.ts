import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-input-field',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './input-field.html',
  styleUrls: ['./input-field.scss'],
})
export class InputFieldComponent {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: 'text' | 'password' | 'email' = 'text';
  @Input() value: any;
  @Output() valueChange = new EventEmitter<any>();
}
