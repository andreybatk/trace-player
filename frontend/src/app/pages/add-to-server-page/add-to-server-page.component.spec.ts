import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddToServerPageComponent } from './add-to-server-page.component';

describe('AddToServerPageComponent', () => {
  let component: AddToServerPageComponent;
  let fixture: ComponentFixture<AddToServerPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddToServerPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddToServerPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
