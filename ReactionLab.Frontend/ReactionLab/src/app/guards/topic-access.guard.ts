import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { ApiService } from '../services/api.service';

export const topicAccessGuard: CanActivateFn = (route) => {
  const apiService = inject(ApiService);
  const router = inject(Router);

  const topicId = Number(route.paramMap.get('topicId'));

  return apiService.getTopics().pipe(
    map(topics => {
      const topic = topics.find(t => t.id === topicId);
      if (!topic || topic.status === 'locked') {
        router.navigate(['/topics']);
        return false;
      }
      return true;
    }),
    catchError(() => {
      router.navigate(['/topics']);
      return of(false);
    })
  );
};
