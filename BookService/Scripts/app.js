var ViewModel = function () {
    var self = this;
    self.books = ko.observableArray();
    self.error = ko.observable();

    //uri's
    var booksUri = '/api/books/';
    var authorsUri = '/api/authors/';

    function ajaxHelper(uri, method, data) {
        //clear error
        self.error('');
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });

    }
    function getAllBooks() {
        ajaxHelper(booksUri, 'GET').done(function (data) {
            self.books(data);
        });
    }

    //Getting book detail
    self.detail = ko.observable();
    self.getBookDetail = function (item) {
        ajaxHelper(booksUri + item.Id, 'GET').done(function (data) {
            self.detail(data)
        });
    }
    //get initial data
    getAllBooks();

    //Adding Book to the database
    self.authors = ko.observableArray();
    self.newbook = {
        Author: ko.observable(),
        Genre: ko.observable(),
        Price: ko.observable(),
        Title: ko.observable(),
        Year: ko.observable()
    }
    //get all authors
    function getAuthors()
    {
        ajaxHelper(authorsUri, 'GET').done(function (data) {
            self.authors(data);

        });
    }

    self.addBook = function (formElement) {
        var book = {
            AuthorId: self.newbook.Author().Id,
            Genre: self.newbook.Genre(),
            Title: self.newbook.Title(),
            Year: self.newbook.Year(),
            Price: self.newbook.Price()
        };
        ajaxHelper(authorsUri, 'POST', book).done(function (item) {
            self.books.push(item);
        });
    }
    getAuthors();

};
ko.applyBindings(new ViewModel());