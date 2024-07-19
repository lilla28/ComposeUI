import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MarketWatchComponent } from './components/market-watch/market-watch.component';

const routes: Routes = [
    {path: '', component: MarketWatchComponent},
    {path: 'market-data', component: MarketWatchComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
