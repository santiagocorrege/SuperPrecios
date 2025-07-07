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
	('Devoto', 'https://www.devoto.com.uy'),
	('Geant', 'https://www.geant.com.uy'),
	('Disco', 'https://www.disco.com.uy')