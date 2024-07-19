import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeIdeaGeneratorComponent } from './trade-idea-generator.component';

describe('TradeIdeaGeneratorComponentComponent', () => {
  let component: TradeIdeaGeneratorComponent;
  let fixture: ComponentFixture<TradeIdeaGeneratorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradeIdeaGeneratorComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TradeIdeaGeneratorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
