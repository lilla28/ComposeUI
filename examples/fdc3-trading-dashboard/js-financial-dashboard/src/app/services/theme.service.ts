import { Injectable, signal, WritableSignal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  themeSignal: WritableSignal<string> = signal<string>("dark");

  public setTheme(theme: string): void {
    this.themeSignal.set(theme);
  }

  public updateTheme() : void {
    this.themeSignal.update(value => value === "dark" ? "light" : "dark");
  }
}
