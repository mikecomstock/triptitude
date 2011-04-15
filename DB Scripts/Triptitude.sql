USE [Triptitude]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [UserTrips](
	[User_Id] [int] NOT NULL,
	[Trip_Id] [int] NOT NULL,
 CONSTRAINT [PK_UserTrips] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC,
	[Trip_Id] ASC
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
	[DefaultTrip_Id] [int] NULL,
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
	[BeginDate] [date] NULL,
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
CREATE TABLE [Notes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItineraryItem_Id] [int] NOT NULL,
	[Created_By] [int] NOT NULL,
	[Created_On] [datetime] NOT NULL,
	[Text] [varchar](2000) NOT NULL,
	[Public] [bit] NOT NULL,
 CONSTRAINT [PK_ItineraryItemNote] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ItineraryItemId] ON [Notes] 
(
	[ItineraryItem_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
SET ANSI_PADDING ON
GO
CREATE TABLE [Hotels](
	[Id] [int] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Latitude] [decimal](9, 6) NOT NULL,
	[Longitude] [decimal](9, 6) NOT NULL,
	[GeoPoint] [geography] NULL,
	[HasContinentalBreakfast] [bit] NOT NULL,
 CONSTRAINT [PK_Hotels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE SPATIAL INDEX [IX_GeoPoint] ON [Hotels] 
(
	[GeoPoint]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Transportations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Trip_Id] [int] NOT NULL,
	[TransportationType_Id] [int] NOT NULL,
	[FromCity_GeoNameId] [int] NOT NULL,
	[ToCity_GeoNameId] [int] NOT NULL,
	[BeginDay] [int] NULL,
	[EndDay] [int] NULL,
 CONSTRAINT [PK_Transportations] PRIMARY KEY CLUSTERED 
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
CREATE TABLE [TransportationTypes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TransportationTypes] PRIMARY KEY CLUSTERED 
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
CREATE TABLE [Tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
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
CREATE TABLE [ScheduledTags](
	[Id] [int] NOT NULL,
	[Tag_Id] [int] NOT NULL,
	[GeoNameId] [int] NOT NULL
) ON [PRIMARY]
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
	[Country_GeoNameID] [int] NOT NULL,
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
CREATE TABLE [HotelPhotos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Hotel_Id] [int] NOT NULL,
	[ImageURL] [varchar](255) NOT NULL,
	[ThumbURL] [varchar](255) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[Height] [int] NOT NULL,
	[Width] [int] NOT NULL,
 CONSTRAINT [PK_HotelPhotos] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [CX_Hotel_Id] ON [HotelPhotos] 
(
	[Hotel_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [InsertHotel]
	@Id [int],
	@Name [varchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@HasContinentalBreakfast [bit]
WITH EXECUTE AS CALLER
AS
begin
	begin tran
		declare @ExistingId int
		select @ExistingId = Id from Hotels with(NOLOCK) where Id = @Id

		if @ExistingId is null
		begin
			insert into Hotels (Id, Name, Latitude, Longitude, GeoPoint, HasContinentalBreakfast) values (@Id, @Name,@Latitude,@Longitude,geography::Point(@Latitude, @Longitude, 4326), @HasContinentalBreakfast)
		end
		else
		begin
			update Hotels set Name = @Name, Latitude = @Latitude, Longitude = @Longitude, GeoPoint = geography::Point(@Latitude, @Longitude, 4326),HasContinentalBreakfast = @HasContinentalBreakfast  where id = @Id
		end
	commit
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [HotelsNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select *,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Hotels
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [InsertHotelPhoto]
	@HotelId [int],
	@ImageURL [varchar](255),
	@ThumbURL [varchar](255),
	@IsDefault [bit],
	@Height [int],
	@Width [int]
WITH EXECUTE AS CALLER
AS
begin
	declare @ExistingHotelId int
	select @ExistingHotelId = Id from Hotels where Id = @HotelId

	if @ExistingHotelId is not null
	begin
		insert into HotelPhotos(Hotel_Id, ImageURL, ThumbURL, IsDefault, Height, Width) values (@HotelId, @ImageURL, @ThumbURL, @IsDefault, @Height, @Width)	
	end
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
	[Region_GeoNameID] [int] NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[GeoNameId] ASC
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
	[Trip_Id] [int] NOT NULL,
	[Website_Id] [int] NULL,
	[BeginDay] [int] NULL,
	[BeginTime] [time](7) NULL,
	[EndDay] [int] NULL,
	[EndTime] [time](7) NULL,
	[Hotel_Id] [int] NULL,
	[DestinationTag_Id] [int] NULL,
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
CREATE TABLE [DestinationTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tag_Id] [int] NOT NULL,
	[City_GeoNameId] [int] NOT NULL,
 CONSTRAINT [PK_TagDestinations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [Destinations] as
select Name, GeoNameId, GeoNameID Country_GeoNameId, null City_GeoNameId, null Region_GeoNameId from Countries
union
select ASCIIName Name, GeoNameId, null Country_GeoNameId, GeoNameID Region_GeoName_Id, null City_GeoNameId  from Regions
union
select ASCIIName Name, GeoNameId, null Country_GeoNameId,null Region_GeoNameId, GeoNameId City_GeoNameId  from cities
GO
ALTER TABLE [UserTrips]  WITH CHECK ADD  CONSTRAINT [FK_UserTrips_Trips] FOREIGN KEY([Trip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [UserTrips] CHECK CONSTRAINT [FK_UserTrips_Trips]
GO
ALTER TABLE [UserTrips]  WITH CHECK ADD  CONSTRAINT [FK_UserTrips_Users] FOREIGN KEY([User_Id])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [UserTrips] CHECK CONSTRAINT [FK_UserTrips_Users]
GO
ALTER TABLE [Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_DefaultTrips] FOREIGN KEY([DefaultTrip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [Users] CHECK CONSTRAINT [FK_Users_DefaultTrips]
GO
ALTER TABLE [Trips]  WITH CHECK ADD  CONSTRAINT [FK_Trips_Created_By] FOREIGN KEY([Created_By])
REFERENCES [Users] ([Id])
GO
ALTER TABLE [Trips] CHECK CONSTRAINT [FK_Trips_Created_By]
GO
ALTER TABLE [Notes]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItemNote_ItineraryItems] FOREIGN KEY([ItineraryItem_Id])
REFERENCES [ItineraryItems] ([Id])
GO
ALTER TABLE [Notes] CHECK CONSTRAINT [FK_ItineraryItemNote_ItineraryItems]
GO
ALTER TABLE [Transportations]  WITH CHECK ADD  CONSTRAINT [FK_Transportations_TransportationTypes] FOREIGN KEY([TransportationType_Id])
REFERENCES [TransportationTypes] ([id])
GO
ALTER TABLE [Transportations] CHECK CONSTRAINT [FK_Transportations_TransportationTypes]
GO
ALTER TABLE [Transportations]  WITH CHECK ADD  CONSTRAINT [FK_Transportations_Trips] FOREIGN KEY([Trip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [Transportations] CHECK CONSTRAINT [FK_Transportations_Trips]
GO
ALTER TABLE [Regions]  WITH CHECK ADD  CONSTRAINT [FK_Regions_Countries] FOREIGN KEY([Country_GeoNameID])
REFERENCES [Countries] ([GeoNameID])
GO
ALTER TABLE [Regions] CHECK CONSTRAINT [FK_Regions_Countries]
GO
ALTER TABLE [HotelPhotos]  WITH CHECK ADD  CONSTRAINT [FK_HotelPhotos_Hotels] FOREIGN KEY([Hotel_Id])
REFERENCES [Hotels] ([Id])
GO
ALTER TABLE [HotelPhotos] CHECK CONSTRAINT [FK_HotelPhotos_Hotels]
GO
ALTER TABLE [Cities]  WITH CHECK ADD  CONSTRAINT [FK_Cities_Regions] FOREIGN KEY([Region_GeoNameID])
REFERENCES [Regions] ([GeoNameID])
GO
ALTER TABLE [Cities] CHECK CONSTRAINT [FK_Cities_Regions]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_Hotels] FOREIGN KEY([Hotel_Id])
REFERENCES [Hotels] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_Hotels]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_TagDestinations] FOREIGN KEY([DestinationTag_Id])
REFERENCES [DestinationTags] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_TagDestinations]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_Trips] FOREIGN KEY([Trip_Id])
REFERENCES [Trips] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_Trips]
GO
ALTER TABLE [ItineraryItems]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItems_Websites] FOREIGN KEY([Website_Id])
REFERENCES [Websites] ([Id])
GO
ALTER TABLE [ItineraryItems] CHECK CONSTRAINT [FK_ItineraryItems_Websites]
GO
ALTER TABLE [DestinationTags]  WITH CHECK ADD  CONSTRAINT [FK_TagDestinations_Cities] FOREIGN KEY([City_GeoNameId])
REFERENCES [Cities] ([GeoNameId])
GO
ALTER TABLE [DestinationTags] CHECK CONSTRAINT [FK_TagDestinations_Cities]
GO
ALTER TABLE [DestinationTags]  WITH CHECK ADD  CONSTRAINT [FK_TagDestinations_Tags] FOREIGN KEY([Tag_Id])
REFERENCES [Tags] ([Id])
GO
ALTER TABLE [DestinationTags] CHECK CONSTRAINT [FK_TagDestinations_Tags]
GO
