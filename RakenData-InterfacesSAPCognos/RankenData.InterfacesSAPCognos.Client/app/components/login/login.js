//(function () {
//    'use strict';

//    angular
//        .module('interface.login',[])
//        .controller('LoginCtrl', controller);

//    controller.$inject = ['$scope']; 

//    function controller($scope) {
//        $scope.title = 'controller';

//        activate();

//        function activate() {
//            $scope.login = function () {
//                alert("Hola Angular");
//            }
//        }
//    }
//})();

angular
.module('interface.login', [])
.controller('LoginCtrl', function ($scope) {
    $scope.nombreCompleto = "Luis Fernando Forero Guzman";
});
