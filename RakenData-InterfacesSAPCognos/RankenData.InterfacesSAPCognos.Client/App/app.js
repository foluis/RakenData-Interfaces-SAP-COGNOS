
//(function () {
//    'use strict';

//    angular.module('interface', []);
//})();



angular.module('interface', [
    'ngRoute',
    'interface.login',
    'interface.home'
])
.config(function ($routeProvider) {
    $routeProvider
        .when('/', {
            controller: 'HomeCtrl',
            templateUrl: 'app/components/home/home.html'
        })
       .when('/login', {
           controller: 'LoginCtrl',
           templateUrl: 'app/components/login/login.html'
       })
});




//(function () {
//    'use strict';

//    angular.module('interface', [
//        // Angular modules 
//        'ngRoute'
//        // Custom modules 
//        , 'interface.login'
//        , 'interface.home'
//        // 3rd Party Modules
//    ]).
//        config(function ($routeProvider) {
//            $routeProvider
//                .when('/', {
//                    controller: 'HomeCtrl',
//                    templateUrl: 'App/components/home/home.html'
//                })
//               .when('/login', {
//                   controller: 'LoginCtrl',
//                   templateUrl: 'App/components/login/login.html'
//               })
//        });
//})();
