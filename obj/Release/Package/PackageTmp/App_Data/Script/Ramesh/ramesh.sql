USE [IOASDB]
GO
/****** Object:  UserDefinedTableType [dbo].[dtDynFilter]    Script Date: 7/30/2018 10:49:47 AM ******/
CREATE TYPE [dbo].[dtDynFilter] AS TABLE(
	[ReportID] [int] NULL,
	[ReportField] [varchar](100) NULL,
	[FieldType] [varchar](100) NULL,
	[RefTable] [varchar](100) NULL,
	[RefField] [varchar](100) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[dtDynReportRoles]    Script Date: 7/30/2018 10:49:47 AM ******/
CREATE TYPE [dbo].[dtDynReportRoles] AS TABLE(
	[ReportID] [int] NULL,
	[RoleId] [varchar](100) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[dtDynSummary]    Script Date: 7/30/2018 10:49:47 AM ******/
CREATE TYPE [dbo].[dtDynSummary] AS TABLE(
	[ReportID] [int] NULL,
	[ReportField] [varchar](100) NULL,
	[Aggregation] [varchar](100) NULL,
	[GroupBy] [bit] NULL,
	[OrderBy] [bit] NULL
)
GO
/****** Object:  Table [dbo].[tblDynamicFilter]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDynamicFilter](
	[ReportID] [int] NULL,
	[ReportField] [varchar](100) NULL,
	[FieldType] [varchar](100) NULL,
	[RefTable] [varchar](100) NULL,
	[RefField] [varchar](100) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDynamicReportRoles]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDynamicReportRoles](
	[ReportID] [int] NULL,
	[RoleId] [int] NULL,
	[IsDeleted] [bit] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblDynamicReports]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDynamicReports](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[ReportName] [varchar](max) NULL,
	[Description] [varchar](max) NULL,
	[TableName] [varchar](max) NULL,
	[Fields] [varchar](max) NOT NULL,
	[GroupByFields] [varchar](max) NULL,
	[OrderByFields] [varchar](max) NULL,
	[RoleID] [int] NULL,
	[ModuleID] [int] NULL,
	[IsActive] [bit] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDynamicSummary]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDynamicSummary](
	[ReportID] [int] NULL,
	[ReportField] [varchar](100) NULL,
	[Aggregation] [varchar](100) NULL,
	[GroupBy] [bit] NULL,
	[OrderBy] [bit] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[tblDynamicReportRoles] ADD  CONSTRAINT [DF_tblDynamicReportRoles_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  StoredProcedure [dbo].[DynamicReportsIU]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[DynamicReportsIU]
(

@ReportID Int OUTPUT,
@ReportName Varchar(50),
@Description Varchar(50),
@TableName Varchar(50),
@Fields Varchar(50),
@GroupByFields Varchar(50),
@OrderByFields Varchar(50),
@IsActive INT,
@UserId Int,
@RoleId Int,
@ModuleId Int,
@IsDeleted bit,
@dtSummary dtDynSummary readonly,
@dtFilter dtDynFilter readonly,
@dtRoles dtDynReportRoles readonly,
@Status int OUTPUT 

)
AS
BEGIN
	select @ReportID as ReportID
	BEGIN TRANSACTION ReportSave
  IF (ISNULL(@ReportID, 0) = 0)
	BEGIN
		
		
		SET NOCOUNT ON;
		set @ReportID = ''
		INSERT INTO tblDynamicReports(ReportName, Description, TableName, Fields, GroupByFields, OrderByFields, 
			IsActive, IsDeleted, CRTD_TS, CRTD_UserID, RoleID, ModuleID)
          
		Values(ISNULL(@ReportName,'') , ISNULL(@Description,''),ISNULL(@TableName,''),ISNULL(@Fields,''),ISNULL(@GroupByFields,''),
			   ISNULL(@OrderByFields,''),ISNULL(@IsActive,0), ISNULL(@IsDeleted,0), GETDATE(), @UserId,@RoleId, @ModuleId)
			  
			  SET  @ReportID =IDENT_CURRENT('tblDynamicReports')
   			   --set @Status='Report '+@ReportName+' created successfully' 
   			   set @Status=1 
   			   
   			   
   			insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   			select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
   			
   			insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField)
   			select @ReportID, ReportField, FieldType, RefTable, RefField from @dtFilter
   			
   			insert into tblDynamicReportRoles(ReportID, RoleId)
   			select @ReportID, RoleId from @dtRoles
   			
   			IF (@@ERROR <> 0)
			BEGIN
				ROLLBACK TRANSACTION ReportSave
				RETURN
			END
   			
	END
 ELSE

	BEGIN
    SET NOCOUNT ON;
		 					
		UPDATE tblDynamicReports
		SET
			ReportName= ISNULL(@ReportName,''),
			TableName=ISNULL(@TableName,''),
			Fields=ISNULL(@Fields,''),
			GroupByFields=ISNULL(@GroupByFields,''),
			OrderByFields=ISNULL(@OrderByFields,''),
			IsActive=ISNULL(@IsActive,''),
			IsDeleted=ISNULL(@IsDeleted,0),
			UPDT_TS=GETDATE(),
			UPDT_UserId=@UserId

		WHERE           
			ReportID=@ReportID 
			
		delete from tblDynamicSummary where ReportID = @ReportID;
		
		insert into tblDynamicSummary(ReportID, ReportField, Aggregation, GroupBy, OrderBy)
   		select @ReportID, ReportField, Aggregation, GroupBy, OrderBy from @dtSummary
			
			
		delete from tblDynamicFilter where ReportID = @ReportID;
		
		insert into tblDynamicFilter(ReportID, ReportField, FieldType, RefTable, RefField)
   		select @ReportID, ReportField, FieldType, RefTable, RefField from @dtFilter
		
		delete from tblDynamicReportRoles where ReportID = @ReportID;
		
		insert into tblDynamicReportRoles(ReportID, RoleId)
		select @ReportID, RoleId from @dtRoles   
							
		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION ReportSave
			RETURN
		END   					
   					
		--set @Status='Report '+@ReportName+' updated successfully' 
		set @Status=2 
	END
		
	
	COMMIT TRANSACTION ReportSave
	
END



GO
/****** Object:  StoredProcedure [dbo].[GetDynamicReport]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GetDynamicReport]
@ReportID INT
AS
	SELECT     ReportID, ReportName, Description, TableName, Fields, GroupByFields, OrderByFields, RoleID, ModuleID, IsActive, CRTD_TS, UPDT_TS, CRTD_UserID, UPDT_UserID, IsDeleted
	FROM         tblDynamicReports WHERE (ReportID = @ReportID OR (@ReportID=-1 AND 1=1))
	
	SELECT     ReportID, ReportField, Aggregation, GroupBy, OrderBy
	FROM         tblDynamicSummary
	WHERE     (ReportID = @ReportID)
	
	SELECT     ReportID, ReportField, FieldType, RefTable, RefField
	FROM         tblDynamicFilter
	WHERE     (ReportID = @ReportID)
	
	SELECT     ReportID, RoleId, IsDeleted
	FROM         tblDynamicReportRoles
	WHERE     (ReportID = @ReportID)



GO
/****** Object:  StoredProcedure [dbo].[GetFieldDetails]    Script Date: 7/30/2018 10:49:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFieldDetails] 
@TableName varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 select C.column_id as ID, C.name, O.DATA_TYPE as datatype, ISNUMERIC(IsNull(O.NUMERIC_PRECISION,'false')) AS IsNumber  from sys.views V 
		inner join INFORMATION_SCHEMA.COLUMNS O on V.name = O.TABLE_NAME
		inner join sys.columns C on V.object_id = C.object_id and C.name = O.COLUMN_NAME
		where v.name = @TableName
END

GO
