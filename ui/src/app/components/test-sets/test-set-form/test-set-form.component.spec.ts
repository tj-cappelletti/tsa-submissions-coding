import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestSetFormComponent } from './test-set-form.component';

describe('TestSetFormComponent', () => {
  let component: TestSetFormComponent;
  let fixture: ComponentFixture<TestSetFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestSetFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestSetFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
