import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TradeIdeaGeneratorComponent } from './components/trade-idea-generator/trade-idea-generator.component';

export const routes: Routes = [
    {path: '', component: TradeIdeaGeneratorComponent},
    {path: 'trade-idea-generator', component: TradeIdeaGeneratorComponent}
];


@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {}