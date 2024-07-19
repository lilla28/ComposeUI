import { Component, inject } from '@angular/core';
import { ThemeService } from '../services/theme.service';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrl: './sidenav.component.scss'
})
export class SidenavComponent {
  public themeService: ThemeService = inject(ThemeService);
  public isSlideChecked: boolean = false;

  public toggleTheme(): void {
    this.themeService.updateTheme();
  }

  public toggleThemeWithButton(): void {
    this.isSlideChecked = !this.isSlideChecked;
    this.toggleTheme();
  }
}
