
import { HttpService } from 'src/app/src/app/services/http.service';

export function appInitializer(httpService: HttpService) {
    return () => new Promise(resolve => {
        // attempt to refresh token on app start up to auto authenticate
        httpService.refreshToken()
            .subscribe(x => console.log('Init'))
            .add(resolve);
    });
}