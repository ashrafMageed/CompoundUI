cmtest.factory('Test1Service', function ($http) {
    return {
        getContent: function (callback) {
            $http.get('http://localhost:98/Page/Index').then(function (response) {
                callback(response.data);
            });
        }
    };
});