import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestSetValueFormComponent } from './test-set-value-form.component';

describe('TestSetValueFormComponent', () => {
  let component: TestSetValueFormComponent;
  let fixture: ComponentFixture<TestSetValueFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestSetValueFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestSetValueFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
