import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TestSetValueModel } from '../../../models/test-set.models';

@Component({
  selector: 'app-test-set-value-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test-set-value-form.component.html',
  styleUrls: ['./test-set-value-form.component.css']
})
export class TestSetValueFormComponent implements OnInit {
  @Input() value: TestSetValueModel | null = null; // If null, it's a create form
  @Output() save = new EventEmitter<TestSetValueModel>();
  @Output() cancel = new EventEmitter<void>();

  id = '';
  dataType = '';
  index = 0;
  isArray = false;
  valueAsJson = '';

  ngOnInit() {
    if (this.value) {
      this.id = this.value.id;
      this.dataType = this.value.dataType;
      this.index = this.value.index;
      this.isArray = this.value.isArray;
      this.valueAsJson = this.value.valueAsJson;
    }
  }

  onSubmit() {
    if (!this.dataType.trim() || !this.valueAsJson.trim()) {
      return;
    }
    const model: TestSetValueModel = {
      id: this.id || crypto.randomUUID(),
      dataType: this.dataType,
      index: this.index,
      isArray: this.isArray,
      valueAsJson: this.valueAsJson
    };
    this.save.emit(model);
  }

  onCancel() {
    this.cancel.emit();
  }
}
