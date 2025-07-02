use SuperPrecios;
GO

INSERT INTO Usuarios VALUES ('Admin', '', 'admin@admin.com', '$2a$12$Mpocyh6Qtixvpa9nbJgWG.ZSIHWd.r844f5KGuz9yh92VaekuOAIm', 'Administrador', NULL)
GO
INSERT INTO Marcas VALUES
('Pepsi'),
('Coca-Cola'),
('Sibarita');
GO
INSERT INTO Supermercados (Nombre, WebsiteUrl) VALUES
	('TA-TA', 'www.testwebA.com'),
	('Tienda Inglesa', 'www.testwebB.com')