	// create the module and name it App
	var App = angular.module('App', ['ngRoute','ngCookies']);

	App.directive('loadingss', ['$http', function ($http) {
		  return {
			  restrict: 'A',
			  link: function (scope, element, attrs) {
				  scope.isLoading = function () {
					  return $http.pendingRequests.length > 0;
				  };
				  scope.$watch(scope.isLoading, function (value) {
					  if (value) {
						  $("#loadingss").animate({top: '0px'},800);
					  } else {
						  $("#loadingss").animate({ top: '-100px' },800);
					  }
				  });
			  }
		  };
	  } ]);

	// configure our routes
	App.config(function($routeProvider) {
		$routeProvider

			// route for the home page
			.when('/', {
				templateUrl : 'pages/home.html',
				controller  : 'HomeController'
			})

			// route for the about page
			.when('/menu', {
				templateUrl : 'pages/menu.html',
				controller  : 'menuController'
			})

			// route for the contact page
			.when('/intovan', {
				templateUrl : 'pages/intovan.html',
				controller  : 'intovanController'
			})
			
			.when('/VANDetails', {
				templateUrl : 'pages/VANDetails.html',
				controller  : 'VANDetailsController'
			})
			
			.when('/intooutlet', {
				templateUrl : 'pages/intooutlet.html',
				controller  : 'intooutletController'
			})
			.when('/BARDetails', {
				templateUrl : 'pages/BARDetails.html',
				controller  : 'BARDetailsController'
			})
			
			.when('/intokitchen', {
				templateUrl : 'pages/intokitchen.html',
				controller  : 'intokitchenController'
			})
			.when('/kitchenDetails', {
				templateUrl : 'pages/kitchenDetails.html',
				controller  : 'kitchenDetailsController'
			})
			
			.otherwise({redirectTo:'/'});
	});

	// create the controller and inject Angular's $scope
	App.controller('mainController', ['$scope', '$rootScope','util_SERVICE', function($scope,$rootScope,US) {
		// create a message to display in our view
		$scope.message = 'Everyone come and see how good I look!';
		$rootScope.loginpage=true;
		
	}]);
	
	// create the controller and inject Angular's $scope
	App.controller('HomeController', ['$scope', '$rootScope','util_SERVICE','$http','$cookies', function($scope,$rootScope,US,$http,$cookies) {
		// create a message to display in our view
			
		$scope.message = 'Everyone come and see how good I look!';
		$rootScope.loginpage=true;
		
		
				$scope.loadcompany = function () {
				var data = {"sUserName" : $scope.userId, "sPassword" : $scope.password, "sCompany" : $scope.company}
				var parms = JSON.stringify(data);
				$http.post(US.url+"MGet_CompanyList", "sJsonInput=" + "", US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.companylist = jsondata;
								 $scope.company = $scope.companylist[0].U_DBName;
							   }
								else
								   $scope.companylist = [];
						   },
						   function (response) { // failure callback
						   }
					);
				}//loadcompany end
				
				$scope.checklogin = function () {
					var data = {"sUserName" : $scope.userId, "sPassword" : $scope.password, "sCompany" : $scope.company}
					var parms = JSON.stringify(data);
					$http.post(US.url+"MGet_Login", "sUserName=" + $scope.userId +"&sPassword="+$scope.password+"&sCompanyDB="+$scope.company, US.config)
						 .then(
							   function (response) {
						   // success callback
						  	parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							xmldata = xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue;
							//var jsondata= JSON.parse();
							console.log(xmldata);
						   if (xmldata != "Your Role is not a Driver" && 
xmldata != "UserName and Password is Incorrect") {
							   var jsondata= JSON.parse(xmldata);
							   //$cookies.put('MenuInfo', JSON.stringify(response.data.MenuInfo));
							   $cookies.put('UserData', JSON.stringify(jsondata));
							   $cookies.put('Islogin', "true");
							   //$cookies.put('UsName', $scope.userId);
							   US.UsName =  $scope.userId;
							   US.UsPass= $scope.password;
							   US.UsDB= $scope.company;
							   $rootScope.rootuser=US.UsName;
								$rootScope.rootDate=US.today;
							   window.location = "#/menu";
						   }
							else
							   alert(xmldata);
						   },
						   function (response) { // failure callback
								}
							);
					
						}//checklogin end

				
				
				$scope.loadcompany();
		
	}]);

	App.controller('menuController', function($scope,$rootScope) {
		$scope.message = 'Look! I am an about page.';
		$rootScope.loginpage=false;
		
		$scope.navpage = function(page)
		{
			window.location = "#/"+page;
		}
	});

	App.controller('intovanController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$scope.CKTHide = false;
		$scope.CS = false;
		$scope.CK = false;
		$scope.supplier = "";
		$scope.OutLet =" --- Select Outlet --- ";
		
		US.GetOutLetList().then(function (response)
										  {
										  console.log($scope.OutLetList)
										  parser = new DOMParser();
										  xmlDoc = parser.parseFromString(response.data,"text/xml");
										  xmldata = xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue;
										  $scope.OutLetList = JSON.parse(xmldata);
						  
										  });
		
		$scope.outletchanged = function ()
		{
			var parms = "sCompanyDB="+US.UsDB+"&sOutlet="+$scope.OutLet;
			$http.post(US.url+"MGet_InventoryRequest", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.VANList = jsondata;
							   }
								else
								   $scope.VANList = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		$scope.GOTOVANetails = function(d)
		{
			$rootScope.ReqNo = d.RequestNo;
			$rootScope.OutletName = $scope.OutLet;
			window.location = "#/VANDetails"
		}
		
		
	}]);
	
	
	//van details 
	App.controller('VANDetailsController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$('.datepicker').datepicker({ dateFormat: 'yy-mm-dd' });
		
		$scope.Username = US.UsName;
		$scope.orderdate = "";
		
		$scope.checkOpenQty = function(r,o,i)
		{
			if(r>o)
			{
				alert("Can't Enter More than Open Qty");
				$scope.VAN_Details[i].ReceiptQty=0;
				return false;
			}
			else
			{
				return true;
			}
		}
		
		$scope.loaddata = function()
		{
			
			var parms = "sCompanyDB="+US.UsDB+"&sRequestNo="+$rootScope.ReqNo;
			$http.post(US.url+"MGet_InventoryRequestDetails", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.VAN_Details = jsondata;
							   }
								else
								   $scope.VAN_Details = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		$scope.loaddata();
		
		$scope.ADD = function()
		{
			if($scope.orderdate=="")
			{
				alert("Kindly Select Date.");
				return false;
			}
			else
			{
				$scope.msg = "Do you want to close the PO Lines ?";
				$('#myModal').modal('show'); 
			}
		}
		
		$scope.AckNow = function()
		{
								
			//console.log($scope.jfile);
			var parms = "sUserName="+$scope.ACKUSERNAME+"&sPassword="+$scope.ACKPASSWORD+"&sOutlet="+$rootScope.OutletName+"&sCompanyDB="+US.UsDB;
			$http.post(US.url+"MGet_UserAcknowledgement", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							console.log(response);
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							//console.log(jsondata);
							   if (jsondata[0].sReturnValue =="SUCCESS") {
								   			$scope.AddData();
								 
							   }
								else
								   alert(jsondata[0].sReturnValue);
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		$scope.AddData = function(status)
		{
			$('#myModal').modal('hide'); 
			$scope.otdata = $scope.buildData("Y");
			$scope.jfile =[{			"CardCode": "",
										"CardName": "",
										"RequestNo":  $rootScope.ReqNo,
										"WhsCode": $rootScope.OutletName,
										"RequestDate": $scope.orderdate,
										"InventoryTransfer": $scope.otdata
									}];
									
									
									
			//console.log($scope.jfile);
			var parms = "sCompanyDB="+US.UsDB+"&sJSONFile="+JSON.stringify($scope.jfile);
			$http.post(US.url+"MGet_ReceiveInVAN", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							console.log(response);
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							//console.log(jsondata);
							   if (jsondata[0].sReturnValue =="SUCCESS") {
								   alert("Inventory Transfer Document Created Successfully.");
									window.location = "#/intovan";
							   }
								else
								   alert(jsondata[0].sReturnValue);
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		$scope.buildData = function(s)
		{
			
			
			for(var i=0;i<$scope.VAN_Details.length;i++)
				{
					if($scope.VAN_Details[i].ReceiptQty == undefined || $scope.VAN_Details[i].ReceiptQty == "")
						$scope.VAN_Details[i].ReceiptQty = 0;
					delete $scope.VAN_Details[i].$$hashKey;
					$scope.VAN_Details[i].CloseStatus = s;
					$scope.VAN_Details[i].DocEntry = "";
					$scope.VAN_Details[i].Dscription = $scope.VAN_Details[i].ItemName;
					$scope.VAN_Details[i].Quantity = $scope.VAN_Details[i].OpenQty;;
					$scope.VAN_Details[i].ImageUrl = "";
					$scope.VAN_Details[i].BatchNum = "";
					$scope.VAN_Details[i].BaseEntry = "";
					$scope.VAN_Details[i].BaseLine = "";
					$scope.VAN_Details[i].LineStatus = "";
					$scope.VAN_Details[i].ReasonCode = "";
					$scope.VAN_Details[i].ReasonName = "";
				}
				
				
				
				return $scope.VAN_Details;
		}
		
		
		
		
	}]);
	
	
	App.controller('intooutletController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$scope.CKTHide = false;
		$scope.CS = false;
		$scope.CK = false;
		$scope.supplier = "";
		$scope.OutLet =" --- Select Outlet --- ";
		
		US.GetOutLetList().then(function (response)
										  {
										  console.log($scope.OutLetList)
										  parser = new DOMParser();
										  xmlDoc = parser.parseFromString(response.data,"text/xml");
										  xmldata = xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue;
										  $scope.OutLetList = JSON.parse(xmldata);
						  
										  });
		
		
		$scope.outletchanged = function()
		{
			if($scope.OutLet=="01CKT")
				$scope.CKTHide = true;
			else
				$scope.CKTHide = false;
		}
		
		$scope.CKclicked = function()
		{
			$scope.CS=false;
			$scope.Type = "CK";
			var supp = encodeURIComponent("");
			var parms = "sCompanyDB="+US.UsDB+"&sSupplier="+supp+"&sSupplierType=I&sAccessType=BAR&sOutlet="+$scope.OutLet;
			$http.post(US.url+"MGet_OpenPOList", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.POList = jsondata;
							   }
								else
								   $scope.POList = [];
						   },
						   function (response) { // failure callback
						   }
					);
					
		}
		$scope.CSclicked = function()
		{
			$scope.CK=false;
			$scope.Type = "CS";
		}
		
		$scope.CSSEARCH = function()
		{
			var supp = encodeURIComponent($scope.supplier+"%");
			var parms = "sCompanyDB="+US.UsDB+"&sSupplier="+supp+"&sSupplierType=E&sAccessType=BAR&sOutlet="+$scope.OutLet;
			$http.post(US.url+"MGet_OpenPOList", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.POList = jsondata;
							   }
								else
								   $scope.POList = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		//GOTOBARDetails
		$scope.GOTOBARDetails = function(d)
		{
			$rootScope.selectedData = d;
			$rootScope.supplierName = d.CardName;
			$rootScope.OutletName = $scope.OutLet;
			$rootScope.AccessType = "BAR";
			$rootScope.CompanyDB = US.UsDB;
			$rootScope.Ctype = $scope.Type;
			
			window.location = "#/BARDetails";
		}
		
		

		
	}]);
	
	
	
	
	
	App.controller('BARDetailsController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$('.datepicker').datepicker({ dateFormat: 'yy-mm-dd' });
		
		$scope.Username = US.UsName;
		$scope.orderdate = "";
		
		$scope.ReasonCode = function()
		{
			
			var parms = "sCompanyDB="+US.UsDB+"&sOutlet="+$rootScope.OutletName;
			$http.post(US.url+"MGet_ReasonCode", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.ReasonCode = jsondata;
							   }
								else
								   $scope.ReasonCode = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		
		
		$scope.loaddata = function()
		{
			
			var parms = "sCompanyDB="+US.UsDB+"&sOutlet="+$rootScope.OutletName+"&sAccessType=BAR&sSupplier="+$rootScope.supplierName;
			$http.post(US.url+"MGet_ReceiveInOutlet_Details", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.Outlet_Details = jsondata;
							   }
								else
								   $scope.Outlet_Details = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		$scope.ReasonCode();
		$scope.loaddata();
		
		$scope.setRcode = function(i,rec)
		{
			$scope.Outlet_Details[i].ReasonCode = rec.Code;
			$scope.Outlet_Details[i].ReasonName = rec.Name;
		}
		
		$scope.ADD = function()
		{
			if($scope.orderdate=="")
			{
				alert("Kindly Select Date.");
				return false;
			}
			
			if($rootScope.Ctype=="CK")
			{
				$scope.msg = "Do you want to close the Inventory Transfer Request Lines ?";
				$('#myModal').modal('show'); 
			}
			else
			{
				$scope.msg = "Do you want to close the PO Lines ?";
				$('#myModal').modal('show'); 
			}
		}
		
		$scope.AddData = function(status)
		{
			$('#myModal').modal('hide'); 
			$scope.otdata = $scope.buildData(status);
			$scope.jfile =[{"CardCode": $rootScope.selectedData.CardCode,
										"CardName": $rootScope.selectedData.CardName,
										"NoOfOpenPO": $rootScope.selectedData.NoOfOpenPO,
										"RequestNo": "",
										"WhsCode": $rootScope.OutletName,
										"CreatedUser": US.UsName,
										"ReceiptDate": $scope.orderdate,
										"GrpoLine": $scope.otdata
									}];
									
									
									
			//console.log($scope.jfile);
			var parms = "sCompanyDB="+US.UsDB+"&sJSONFile="+JSON.stringify($scope.jfile);
			$http.post(US.url+"MGet_ReceiveInOutlet", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							console.log(response);
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							//console.log(jsondata);
							   if (jsondata[0].sReturnValue =="SUCCESS") {
								   if($rootScope.Ctype=="CK")
										{
											alert("Inventory Transfer Document Created Successfully.");
											window.location = "#/intooutlet";
										}
										else
										{
											alert("GRPO Document is Created Successfully.");
											window.location = "#/intooutlet";
										}
								 
							   }
								else
								   alert(jsondata[0].sReturnValue);
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		$scope.buildData = function(s)
		{
			
			
			for(var i=0;i<$scope.Outlet_Details.length;i++)
				{
					delete $scope.Outlet_Details[i].ResonCode;
					delete $scope.Outlet_Details[i].$$hashKey;
					$scope.Outlet_Details[i].CloseStatus = s;
				}
				
				
				
				return $scope.Outlet_Details;
		}
		
		
		
		
	}]);
	
	
	//into kitch controller 
	
	App.controller('intokitchenController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$scope.CKTHide = false;
		$scope.CS = false;
		$scope.CK = false;
		$scope.supplier = "";
		$scope.OutLet =" --- Select Outlet --- ";
		
		US.GetOutLetList().then(function (response)
										  {
										  console.log($scope.OutLetList)
										  parser = new DOMParser();
										  xmlDoc = parser.parseFromString(response.data,"text/xml");
										  xmldata = xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue;
										  $scope.OutLetList = JSON.parse(xmldata);
						  
										  });
		
		
		$scope.outletchanged = function()
		{
			if($scope.OutLet=="01CKT")
				$scope.CKTHide = true;
			else
				$scope.CKTHide = false;
		}
		
		$scope.CKclicked = function()
		{
			$scope.CS=false;
			$scope.Type = "CK";
			var supp = encodeURIComponent("");
			var parms = "sCompanyDB="+US.UsDB+"&sSupplier="+supp+"&sSupplierType=I&sAccessType=KITCHEN&sOutlet="+$scope.OutLet;
			$http.post(US.url+"MGet_OpenPOList", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.POList = jsondata;
							   }
								else
								   $scope.POList = [];
						   },
						   function (response) { // failure callback
						   }
					);
					
		}
		$scope.CSclicked = function()
		{
			$scope.CK=false;
			$scope.Type = "CS";
		}
		
		$scope.CSSEARCH = function()
		{
			var supp = encodeURIComponent($scope.supplier+"%");
			var parms = "sCompanyDB="+US.UsDB+"&sSupplier="+supp+"&sSupplierType=E&sAccessType=KITCHEN&sOutlet="+$scope.OutLet;
			$http.post(US.url+"MGet_OpenPOList", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.POList = jsondata;
							   }
								else
								   $scope.POList = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		//GOTOBARDetails
		$scope.GOTOKITCHENDetails = function(d)
		{
			$rootScope.selectedData = d;
			$rootScope.supplierName = d.CardName;
			$rootScope.OutletName = $scope.OutLet;
			$rootScope.AccessType = "KITCHEN";
			$rootScope.CompanyDB = US.UsDB;
			$rootScope.Ctype = $scope.Type;
			
			window.location = "#/kitchenDetails";
		}
		
		

		
	}]);
	
	
	
	
	
	
	
	//in to kitchen 
	
	
	App.controller('kitchenDetailsController', ['$scope', '$rootScope','util_SERVICE','$http','$cookieStore', function($scope,$rootScope,US,$http,$cookie) {
		$scope.message = 'Contact us! JK. This is just a demo.';
		$rootScope.loginpage=false;
		$('.datepicker').datepicker({ dateFormat: 'yy-mm-dd' });
		
		$scope.Username = US.UsName;
		$scope.orderdate = "";
		
		$scope.ReasonCode = function()
		{
			
			var parms = "sCompanyDB="+US.UsDB+"&sOutlet="+$rootScope.OutletName;
			$http.post(US.url+"MGet_ReasonCode", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.ReasonCode = jsondata;
							   }
								else
								   $scope.ReasonCode = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		
		
		$scope.loaddata = function()
		{
			
			var parms = "sCompanyDB="+US.UsDB+"&sOutlet="+$rootScope.OutletName+"&sAccessType=KITCHEN&sSupplier="+$rootScope.supplierName;
			$http.post(US.url+"MGet_ReceiveInOutlet_Details", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							   if (jsondata !="") {
								 $scope.Outlet_Details = jsondata;
							   }
								else
								   $scope.Outlet_Details = [];
						   },
						   function (response) { // failure callback
						   }
					);
		}
		$scope.ReasonCode();
		$scope.loaddata();
		
		$scope.setRcode = function(i,rec)
		{
			$scope.Outlet_Details[i].ReasonCode = rec.Code;
			$scope.Outlet_Details[i].ReasonName = rec.Name;
		}
		
		$scope.ADD = function()
		{
			if($scope.orderdate=="")
			{
				alert("Kindly Select Date.");
				return false;
			}
			
			if($rootScope.Ctype=="CK")
			{
				$scope.msg = "Do you want to close the Inventory Transfer Request Lines ?";
				$('#myModal').modal('show'); 
			}
			else
			{
				$scope.msg = "Do you want to close the PO Lines ?";
				$('#myModal').modal('show'); 
			}
		}
		
		$scope.AddData = function(status)
		{
			$('#myModal').modal('hide'); 
			$scope.otdata = $scope.buildData(status);
			$scope.jfile =[{"CardCode": $rootScope.selectedData.CardCode,
										"CardName": $rootScope.selectedData.CardName,
										"NoOfOpenPO": $rootScope.selectedData.NoOfOpenPO,
										"RequestNo": "",
										"WhsCode": $rootScope.OutletName,
										"CreatedUser": US.UsName,
										"ReceiptDate": $scope.orderdate,
										"GrpoLine": $scope.otdata
									}];
									
									
									
			//console.log($scope.jfile);
			var parms = "sCompanyDB="+US.UsDB+"&sJSONFile="+JSON.stringify($scope.jfile);
			$http.post(US.url+"MGet_ReceiveInOutlet", parms, US.config)
		   			.then(
							function (response) {
							// success callback
							console.log(response);
							parser = new DOMParser();
							xmlDoc = parser.parseFromString(response.data,"text/xml");
							//console.log(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							var jsondata= JSON.parse(xmlDoc.getElementsByTagName("string")[0].childNodes[0].nodeValue);
							//console.log(jsondata);
							   if (jsondata[0].sReturnValue =="SUCCESS") {
								   if($rootScope.Ctype=="CK")
										{
											alert("Inventory Transfer Document Created Successfully.");
											window.location = "#/intokitchen";
										}
										else
										{
											alert("GRPO Document is Created Successfully.");
											window.location = "#/intokitchen";
										}
								 
							   }
								else
								   alert(jsondata[0].sReturnValue);
						   },
						   function (response) { // failure callback
						   }
					);
		}
		
		$scope.buildData = function(s)
		{
			
			
			for(var i=0;i<$scope.Outlet_Details.length;i++)
				{
					delete $scope.Outlet_Details[i].ResonCode;
					delete $scope.Outlet_Details[i].$$hashKey;
					$scope.Outlet_Details[i].CloseStatus = s;
				}
				
				
				
				return $scope.Outlet_Details;
		}
		
		
		
		
	}]);