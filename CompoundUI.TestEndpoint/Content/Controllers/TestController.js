cmtest.controller('Test1Controller', function ($scope, TestService, $sce, $compile) {

    var setContent = function (data) {
        var html = $sce.trustAsHtml(data);
        var module = document.getElementById('content1');
        var CMAPP = angular.module('test', []);
        module.innerHTML = html;
        angular.element(document).ready(function () {
            angular.bootstrap(module, ['test']);
        });

    };
    $scope.getContent = function () {
        TestService.getContent(setContent);
    };

});