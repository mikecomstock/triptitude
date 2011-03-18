USE [Triptitude]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [UserTrips](
	[UserId] [int] NOT NULL,
	[TripId] [int] NOT NULL,
 CONSTRAINT [PK_UserTrips] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[TripId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](200) NOT NULL,
	[DefaultTripId] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Trips](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[Created_By] [int] NOT NULL,
	[Created_On] [datetime] NOT NULL,
 CONSTRAINT [PK_Trips] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [ItineraryItemNotes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItineraryItemId] [int] NOT NULL,
	[Created_By] [int] NOT NULL,
	[Created_On] [datetime] NOT NULL,
	[Text] [varchar](500) NOT NULL,
	[Public] [bit] NOT NULL,
 CONSTRAINT [PK_ItineraryItemNote] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ItineraryItemId] ON [ItineraryItemNotes] 
(
	[ItineraryItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [MasterRecord](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_MasterRecord] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Websites](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[URL] [varchar](1000) NOT NULL,
	[Title] [varchar](200) NOT NULL,
 CONSTRAINT [PK_Websites] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Countries](
	[GeoNameID] [int] NOT NULL,
	[ISO] [varchar](2) NOT NULL,
	[ISO3] [varchar](3) NOT NULL,
	[ISONumeric] [int] NOT NULL,
	[FIPS] [varchar](2) NULL,
	[Name] [varchar](50) NOT NULL,
	[Capital] [varchar](50) NOT NULL,
	[Population] [int] NOT NULL,
	[Continent] [varchar](3) NOT NULL,
	[TLD] [varchar](3) NOT NULL,
	[CurrencyCode] [varchar](3) NOT NULL,
	[CurrencyName] [varchar](50) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[PostalCodeFormat] [varchar](500) NOT NULL,
	[PostalCodeRegex] [varchar](500) NOT NULL,
	[Languages] [varchar](200) NOT NULL,
	[Neighbours] [varchar](50) NOT NULL,
	[EquivalentFipsCode] [varchar](2) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[GeoNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ItineraryItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TripId] [int] NOT NULL,
	[BaseItemId] [int] NULL,
	[WebsiteId] [int] NULL,
	[BeginDay] [int] NULL,
	[BeginTime] [time](7) NULL,
	[EndDay] [int] NULL,
	[EndTime] [time](7) NULL,
	[SoftDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ItineraryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [BaseItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[ItemType] [char](1) NOT NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
 CONSTRAINT [PK_BaseItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Regions](
	[GeoNameID] [int] NOT NULL,
	[ASCIIName] [varchar](200) NOT NULL,
	[GeoNameAdmin1Code] [varchar](20) NULL,
	[CountryGeoNameID] [int] NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[GeoNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [BaseItemPhotos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BaseItemId] [int] NOT NULL,
	[ImageURL] [varchar](255) NOT NULL,
	[ThumbURL] [varchar](255) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
 CONSTRAINT [PK_BaseItemPhoto] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [CX_BaseItemId] ON [BaseItemPhotos] 
(
	[BaseItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ExpediaHotels](
	[ExpediaHotelId] [int] NOT NULL,
	[BaseItemId] [int] NOT NULL,
	[HasContinentalBreakfast] [bit] NOT NULL,
 CONSTRAINT [PK_ExpediaHotels] PRIMARY KEY CLUSTERED 
(
	[ExpediaHotelId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BaseItemId] ON [ExpediaHotels] 
(
	[BaseItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [insert_eh_photo]
	@ExpediaHotelId [int],
	@ImageURL [varchar](255),
	@ThumbURL [varchar](255),
	@IsDefault [bit],
	@Height [int],
	@Width [int]
WITH EXECUTE AS CALLER
AS
begin
	declare @BaseItemId int
	select @BaseItemId = BaseItemId from ExpediaHotels where ExpediaHotelId = @ExpediaHotelId

	if @BaseItemId is not null
	begin
		insert into BaseItemPhotos (BaseItemId, ImageURL, ThumbURL, IsDefault, Height, Width) values (@BaseItemId, @ImageURL, @ThumbURL, @IsDefault, @Height, @Width)	
	end
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [insert_eh]
	@Name [varchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@ExpediaHotelId [int],
	@HasContinentalBreakfast [bit]
WITH EXECUTE AS CALLER
AS
begin
	begin tran
		declare @BaseItemId int
		select @BaseItemId = BaseItemId from ExpediaHotels with(NOLOCK) where ExpediaHotelId = @ExpediaHotelId

		if @BaseItemId is not null
		begin
			update BaseItems set Name = @Name, Latitude = @Latitude, Longitude = @Longitude where id = @BaseItemId
			update ExpediaHotels set HasContinentalBreakfast = @HasContinentalBreakfast where expediahotelid = @ExpediaHotelId
		end
		else
		begin
			insert into BaseItems (Name, ItemType, Latitude, Longitude) values (@Name,'H',@Latitude,@Longitude)
			insert into ExpediaHotels values (@ExpediaHotelId, @@IDENTITY, @HasContinentalBreakfast)
		end
	commit
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Cities](
	[GeoNameId] [int] NOT NULL,
	[ASCIIName] [varchar](200) NOT NULL,
	[Latitude] [decimal](9, 6) NOT NULL,
	[Longitude] [decimal](9, 6) NOT NULL,
	[RegionGeoNameID] [int] NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [ItineraryItems] ADD  CONSTRAINT [DF_ItineraryItems_SoftDeleted]  DEFAULT ((0)) FOR [SoftDeleted]
GO
ALTER TABLE [BaseItemPhotos]  WITH CHECK ADD  CONSTRAINT [FK_BaseItemPhoto_BaseItems] FOREIGN KEY([BaseItemId])
REFERENCES [BaseItems] ([Id])
GO
ALTER TABLE [BaseItemPhotos] CHECK CONSTRAINT [FK_BaseItemPhoto_BaseItems]
GO
ALTER TABLE [Cities]  WITH CHECK ADD  CONSTRAINT [FK_Cities_Regions] FOREIGN KEY([RegionGeoNameID])
REFERENCES [Regions] ([GeoNameID])
GO
ALTER TABLE [Cities] CHECK CONSTRAINT [FK_Cities_Regions]
GO
ALTER TABLE [ExpediaHotels]  WITH CHECK ADD  CONSTRAINT [FK_ExpediaHotels_BaseItems] FOREIGN KEY([BaseItemId])
REFERENCES [BaseItems] ([Id])
GO
ALTER TABLE [ExpediaHotels] CHECK CONSTRAINT [FK_ExpediaHotels_BaseItems]
GO
ALTER TABLE [ItineraryItemNotes]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItemNote_ItineraryItems] FOREIGN KEY([ItineraryItemId])
REFERENCES [ItineraryItems] ([Id])
GO
ALTER TABLE [ItineraryItemNotes] CHECK CONSTRAINT [FK_ItineraryItemNote_ItineraryItems]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_BaseItems] FOREIGN KEY([BaseItemId])
REFERENCES [BaseItems] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_BaseItems]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_Trips] FOREIGN KEY([TripId])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_Trips]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_Websites] FOREIGN KEY([WebsiteId])
REFERENCES [Websites] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_Websites]
GO
ALTER TABLE [Regions]  WITH CHECK ADD  CONSTRAINT [FK_Regions_Countries] FOREIGN KEY([CountryGeoNameID])
REFERENCES [Countries] ([GeoNameID])
GO
ALTER TABLE [Regions] CHECK CONSTRAINT [FK_Regions_Countries]
GO
ALTER TABLE [Trips]  WITH CHECK ADD  CONSTRAINT [FK_Trips_Created_By] FOREIGN KEY([Created_By])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [Trips] CHECK CONSTRAINT [FK_Trips_Created_By]
GO
ALTER TABLE [Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_DefaultTrips] FOREIGN KEY([DefaultTripId])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [Users] CHECK CONSTRAINT [FK_Users_DefaultTrips]
GO
ALTER TABLE [UserTrips]  WITH CHECK ADD  CONSTRAINT [FK_UserTrips_Trips] FOREIGN KEY([TripId])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [UserTrips] CHECK CONSTRAINT [FK_UserTrips_Trips]
GO
ALTER TABLE [UserTrips]  WITH CHECK ADD  CONSTRAINT [FK_UserTrips_Users] FOREIGN KEY([UserId])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [UserTrips] CHECK CONSTRAINT [FK_UserTrips_Users]
GO
