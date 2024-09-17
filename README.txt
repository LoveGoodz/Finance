FINANS PROJESI

Finans projesi iki kısımdan oluşuyor:

1) Backend: .NET 6 ve C# kullanılarak geliştirildi. Bu API, finansal işlemler, müşteri ve fatura gibi temel işlemleri sağlar.
2) Frontend: Vue3 ve PrimeVue kullanılarak geliştirildi. Kullanıcılar için bir arayüz sağlar, faturaları ve müşterileri yönetmeye olanak tanır.


Gereksinimler

	Backend için:
		.NET SDK(6.0)
		SQL Server
		Redis
	Frontend için:
		Node.js(16.0)
		Vue CLI

Backend Kurulumu

.NET Core6, Entity Framework Core ve JWT tabanlı kimlik doğrulama kullanılarak geliştirilmiştir. Bu API, faturalar, müşteriler ve işlemler gibi temel finansal işlemleri destekler. Redis ile cacheleme işlemlerini, Serilog ile loglama işlemlerini sağlar. Swagger ile görüntüleme yapılır.

	1) Projeyi klonlayın:
		git clone https://github.com/LoveGoodz/Finance.git
		cd finance

	2) .NET SDK paketlerini yükleyin:
		dotnet restore

	3) Veritabanını ayarlayın:
		appsetting.josn dosyasındaki SQL server bağlantısı dizgisini düzenleyin: "ConnectionStrings": {
			DefaultConnection": "Server=localhost;Database=FinanceDB;Trusted_Connection=True;"}
		Veritabanı için migration işlemini çalıştırın: dotnet ef database uptade

	4) Redis'i çalıştırın:
		redis-server

	5) API'yi çalıştırın:
		dotnet run

	6) API'yi test edin:
		https://localhost:7093/swagger/index.html

Frontend Kurulumu

Vue3, PrimeVue ve Axios ile geliştirilen web tabanlı bir kullanıcı arayüzüdür. Müşteri yöneti, fatura işlemleri ve diğer finansal işlemleri yapabilir. Backend ile etkileşim kurmak için Axios kullanır ve JWT kimlik doğrulama desteklenir.

	1) Projeyi klonlayın:
		git clone https://github.com/LoveGoodz/Finance.git
		cd finance-ui
	
	2) Gerekli paketleri yükleyin:
		npm install

	3) Axios ile yapılandırmayı ayarlayın:
		axios.defaults.baseURL = 'https://localhost:7093/api';

	4) Projeyi çalıştırın:
		npm run serve

	5) Frontend arayüzüne erişin:
		http:/localhost:8000

Özellik ve Kullanım

	Kimlik Doğrulama(Login Sayfası)
		Kullanıcı, Login sayfasında kullanıcı adı olarak "admin", şifre olarak "123456" kullanarak giriş yapabilir. Başarılı girişten sonra InvoiceList sayfasına yönlendirilir.

	
	Müşteri Yönetimi
		Müşteri ekelem için CustomerCreate sayfasını gidilir(/Customer/Create). Müşteri eklendikten sonra müşterileri listelemek için CustomerList sayfasını kullanabilirsiniz(/Customer)
	
	Fatura Yönetimi
		Yeni fatura eklemek için InvoiceCreate sayfası kullanılır(/Invoice/Create). Eklenen faturalar InvoiceList sayfasında görüntülenir(/Invoice). Her faturanın detayına ulaşmak için InvoiceDetails sayfası kullanılır(/Invoice/ID)

		