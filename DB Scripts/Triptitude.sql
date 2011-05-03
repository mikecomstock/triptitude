USE [Triptitude]
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
	[HashedPassword] [varchar](200) NULL,
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
	[ShowInSiteMap] [bit] NOT NULL,
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
CREATE TABLE [Transportations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Trip_Id] [int] NOT NULL,
	[TransportationType_Id] [int] NOT NULL,
	[FromCity_GeoNameId] [int] NOT NULL,
	[ToCity_GeoNameId] [int] NOT NULL,
	[BeginDay] [int] NOT NULL,
	[EndDay] [int] NOT NULL,
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
SET ANSI_PADDING ON
GO
CREATE TABLE [SignUps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[TripName] [varchar](200) NOT NULL,
	[GeoNameId] [int] NOT NULL,
	[IP] [varchar](200) NULL,
	[RequestInfo] [varchar](4000) NULL,
 CONSTRAINT [PK_SignUps] PRIMARY KEY CLUSTERED 
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
	[Country_GeoNameId] [int] NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[GeoNameID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Country_GeoNameId] ON [Regions] 
(
	[Country_GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Hotels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HotelsCombined_Id] [int] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Latitude] [decimal](9, 6) NOT NULL,
	[Longitude] [decimal](9, 6) NOT NULL,
	[GeoPoint] [geography] NOT NULL,
	[Image_Id] [int] NOT NULL,
	[NumberOfReviews] [int] NOT NULL,
	[ConsumerRating] [decimal](5, 2) NOT NULL,
	[NearestCity_GeoNameId] [int] NOT NULL,
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
CREATE UNIQUE NONCLUSTERED INDEX [UX_HotelsCombined_Id] ON [Hotels] 
(
	[HotelsCombined_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
	[Region_GeoNameId] [int] NOT NULL,
	[GeoPoint] [geography] NOT NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Region_GeoNameId] ON [Cities] 
(
	[Region_GeoNameId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE SPATIAL INDEX [SX_GeoPoint] ON [Cities] 
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
SET ANSI_PADDING ON
GO
CREATE TABLE [Countries](
	[GeoNameID] [int] NOT NULL,
	[ISO] [varchar](2) NOT NULL,
	[Name] [varchar](50) NOT NULL,
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
CREATE PROCEDURE [InsertCity]
	@GeoNameId [int],
	@ASCIIName [nvarchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@Region_GeoNameId [int]
WITH EXECUTE AS CALLER
AS
begin
	insert into Cities
	(GeoNameId, ASCIIName, Latitude, Longitude, GeoPoint, Region_GeoNameId)
	values (@GeoNameId, @ASCIIName, @Latitude, @Longitude, geography::Point(@Latitude, @Longitude, 4326), @Region_GeoNameId)
end
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [HotelsNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select id Hotel_Id,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Hotels
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [CitiesNear](@Latitude [decimal](9, 6), @Longitude [decimal](9, 6), @RadiusInMeters [int])
RETURNS TABLE AS 
return
	select geonameid City_GeoNameId,GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) Distance from Cities
	where GeoPoint.STDistance(geography::Point(@Latitude, @Longitude,4326)) <= @RadiusInMeters
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ItineraryItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Trip_Id] [int] NOT NULL,
	[Website_Id] [int] NULL,
	[BeginDay] [int] NOT NULL,
	[BeginTime] [time](7) NULL,
	[EndDay] [int] NOT NULL,
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
select GeoNameId Id, Name, GeoNameID Country_GeoNameId, null Region_GeoNameId, null City_GeoNameId from Countries
union
select  GeoNameId Id, ASCIIName Name, null Country_GeoNameId, GeoNameID Region_GeoNameId, null City_GeoNameId  from Regions
union
select GeoNameId Id, ASCIIName Name, null Country_GeoNameId,null Region_GeoNameId, GeoNameId City_GeoNameId  from cities
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [InsertHotel]
	@HotelsCombined_Id [int],
	@Name [varchar](200),
	@Latitude [decimal](9, 6),
	@Longitude [decimal](9, 6),
	@Image_Id [int],
	@NumberOfReviews [int],
	@ConsumerRating [decimal](5, 2)
WITH EXECUTE AS CALLER
AS
begin
	begin tran
		declare @ExistingId int
		select @ExistingId = Id from Hotels with(NOLOCK) where HotelsCombined_Id = @HotelsCombined_Id

		declare @NearestCityGeoNameId int
		set @NearestCityGeoNameId  = (select top 1 city_geonameid NearestCityGeoNameId from CitiesNear(@Latitude, @Longitude, 5000) order by distance)

		if @ExistingId is null
		begin
			insert into Hotels (HotelsCombined_Id, Name, Latitude, Longitude, GeoPoint, Image_Id, NumberOfReviews, ConsumerRating, NearestCity_GeoNameId)
			values (@HotelsCombined_Id, @Name,@Latitude,@Longitude,geography::Point(@Latitude, @Longitude, 4326), @Image_Id, @NumberOfReviews, @ConsumerRating, @NearestCityGeoNameId)
		end
		else
		begin
			update Hotels set
			Name = @Name,
			Latitude = @Latitude,
			Longitude = @Longitude,
			GeoPoint = geography::Point(@Latitude, @Longitude, 4326),
			Image_Id = @Image_Id,
			NumberOfReviews = @NumberOfReviews,
			ConsumerRating = @ConsumerRating
			where HotelsCombined_Id = @HotelsCombined_Id
		end
	commit
end
GO
ALTER TABLE [Trips] ADD  CONSTRAINT [DF_Trips_ShowInSitemap]  DEFAULT ((0)) FOR [ShowInSiteMap]
GO
ALTER TABLE [Notes]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryItemNote_ItineraryItems] FOREIGN KEY([ItineraryItem_Id])
REFERENCES [ItineraryItems] ([Id])
GO
ALTER TABLE [Notes] CHECK CONSTRAINT [FK_ItineraryItemNote_ItineraryItems]
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
ALTER TABLE [DestinationTags]  WITH CHECK ADD  CONSTRAINT [FK_TagDestinations_Tags] FOREIGN KEY([Tag_Id])
REFERENCES [Tags] ([Id])
GO
ALTER TABLE [DestinationTags] CHECK CONSTRAINT [FK_TagDestinations_Tags]
GO
