import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Roles } from '../models/auth.models';

export const judgeRoleGuard: CanActivateFn = (route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isAuthenticated()) return router.createUrlTree(['/login']);

    if (auth.getUserRole() === Roles.Judge) {
        return true;
    } else {
        return router.createUrlTree(['/dashboard']);
    }
};