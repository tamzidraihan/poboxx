
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: localhost    Database: smartbox
-- ------------------------------------------------------
-- Server version	8.0.33-0ubuntu0.20.04.2

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Temporary view structure for view `vw_locker_booking_locker`
--

DROP TABLE IF EXISTS `vw_locker_booking_locker`;
/*!50001 DROP VIEW IF EXISTS `vw_locker_booking_locker`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_locker_booking_locker` AS SELECT 
 1 AS `LockerTransactionsId`,
 1 AS `UserKeyId`,
 1 AS `CompanyId`,
 1 AS `LockerDetailId`,
 1 AS `StoragePeriodStart`,
 1 AS `StoragePeriodEnd`,
 1 AS `NewStoragePeriodEndDate`,
 1 AS `SenderName`,
 1 AS `SenderMobile`,
 1 AS `SenderEmailAddress`,
 1 AS `PackageImage`,
 1 AS `DropOffDate`,
 1 AS `DropOffCode`,
 1 AS `DropOffQRCode`,
 1 AS `PickupDate`,
 1 AS `PickUpCode`,
 1 AS `PickUpQRCode`,
 1 AS `ReceiverName`,
 1 AS `ReceiverEmailAddress`,
 1 AS `ReceiverPhoneNumber`,
 1 AS `TotalPrice`,
 1 AS `PaymentMethodId`,
 1 AS `PaymentReference`,
 1 AS `BookingStatus`,
 1 AS `AccessPlan`,
 1 AS `DateCreated`,
 1 AS `DateModified`,
 1 AS `LockerNumber`,
 1 AS `BoardNumber`,
 1 AS `OpenCommand`,
 1 AS `GetStatusCommand`,
 1 AS `IsSubscriptionBooking`,
 1 AS `CabinetLocationDescription`*/;
SET character_set_client = @saved_cs_client;

--
-- Dumping events for database 'smartbox'
--

--
-- Dumping routines for database 'smartbox'
--
/*!50003 DROP PROCEDURE IF EXISTS `sp_ActiveBookingCount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_ActiveBookingCount`(lockerDetailId int, startDate datetime,
    endDate datetime,lockerTransactionId int)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
SET @SelectStatement = 'Select count(l.lockerTransactionsId) from locker_bookings l where (l.BookingStatus=1 or l.BookingStatus=2)';


IF lockerDetailId IS NOT NULL THEN
SET @SelectStatement = CONCAT(@SelectStatement , ' AND l.LockerDetailId=', lockerDetailId);
END IF;
IF lockerTransactionId IS NOT NULL THEN
SET @SelectStatement = CONCAT(@SelectStatement , ' AND l.LockerTransactionsId<>', lockerTransactionId);
END IF;

IF startDate IS NOT NULL AND endDate IS NOT NULL THEN
    SET @SelectStatement = CONCAT(@SelectStatement ," AND ((l.StoragePeriodStart  BETWEEN ", "'",startDate,"'" , " AND ", "'",endDate,"'",")");
    SET @SelectStatement = CONCAT(@SelectStatement ," OR (l.StoragePeriodEnd  BETWEEN ", "'",startDate,"'" , " AND ", "'",endDate,"'",")");
	 SET @SelectStatement = CONCAT(@SelectStatement ," OR (l.StoragePeriodStart >=","'", startDate,"'"," AND l.StoragePeriodEnd<=","'", endDate,"'","))");
END IF;
PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_ActiveLockerBooking` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_ActiveLockerBooking`(lockerTransactionId int)
BEGIN
Select l.lockerTransactionsId,l.LockerDetailId,l.StoragePeriodStart,l.StoragePeriodEnd,ut.DeviceType,ut.Token,u.Email,ld.LockerNumber
,cl.Description as CabinetLocationDescription,l.IsSubscriptionBooking,l.PaymentReference,l.BookingStatus,l.DropOffCode,l.PickUpCode from locker_bookings l
join locker_detail ld on ld.LockerDetailId=l.LockerDetailId
join cabinets c on ld.CabinetId=c.CabinetId
join cabinet_locations cl ON c.CabinetLocationId = cl.CabinetLocationId
join users u on l.UserKeyId=u.UserKeyId
left join user_token ut on ut.UserId = u.UserId
 where (l.BookingStatus=1 or l.BookingStatus=2) 
 and u.IsDeleted=0 and (ut.IsEnable=1 or ut.IsEnable is null) and (l.IsNotified=FALSE or l.IsNotified is Null) 
 and l.lockerTransactionsId=lockerTransactionId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_BookingDetailWithPayments` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_BookingDetailWithPayments`(startDate datetime,endDate datetime,
userKeyid varchar(16),bookingStatus int,currentPage int,
    pageSize int,isActive bool)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(1000);
SET @SelectStatement = 'Select lb.LockerTransactionsId,lb.LockerDetailId,ld.LockerNumber,lb.UserKeyId,lb.ReceiverName,
lb.ReceiverEmailAddress,lb.ReceiverPhoneNumber,lb.DropOffCode,lb.DropOffQRCode,lb.PickUpCode,lb.PickUpQRCode,
lb.TotalPrice,lb.PaymentMethodId,lb.PaymentReference,lb.BookingStatus,lb.StoragePeriodStart,lb.StoragePeriodEnd,
lb.IsSubscriptionBooking,lb.CancelledDate,pt.Type,pt.Status,pt.Amount,pt.InternalType,pt.InternalStatus,c.CabinetId,c.CabinetLocationId,
cl.Description as CabinetLocationDescription,cl.Address as CabinetLocationAddress,u.FirstName as UserFirstName,u.LastName as UserLastName
 from locker_bookings lb join Locker_Detail ld on lb.LockerDetailId=ld.LockerDetailId
 join cabinets c on ld.CabinetId=c.CabinetId
  join cabinet_locations cl on c.CabinetLocationId=cl.CabinetLocationId
  join users u on lb.UserKeyId=u.UserKeyId
 left join payment_transaction pt on (lb.PaymentReference=pt.TransactionId OR lb.lockerTransactionsId=pt.lockerTransactionsId)';
 SET @WhereClause = '';
 IF startDate IS NOT NULL and endDate IS NOT NULL THEN
 SET @WhereClause = CONCAT(@WhereClause ,"(", "'",startDate,"'" , " > lb.StoragePeriodStart AND ", "'",startDate,"'", "< lb.StoragePeriodEnd",")");
  SET @WhereClause = CONCAT(@WhereClause ," OR (", "'",endDate,"'" , " > lb.StoragePeriodStart AND ", "'",endDate,"'", "< lb.StoragePeriodEnd",")");
 END IF;
 
 IF userKeyid IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , " lb.userKeyid='", userKeyid,"'");
END IF;
IF isActive IS NOT NULL AND isActive is True THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , " (lb.BookingStatus=1 or lb.BookingStatus=2) ");
END IF;

 IF bookingStatus IS NOT NULL AND (isActive is null Or isActive is false) THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , " lb.BookingStatus=", bookingStatus);
END IF;

 IF CHAR_LENGTH(@WhereClause) > 0 THEN
 SET @SelectStatement = CONCAT(@SelectStatement,' WHERE ', @WhereClause);
 End If;
 
 IF pageSize IS NOT NULL AND currentPage IS NOT NULL
THEN
SET @v_offset = (currentPage - 1)* pageSize;
    set @SelectStatement = CONCAT(@SelectStatement  , ' LIMIT ',@v_offset,',',PageSize);
END IF;
 PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_ExpiredLockerBookings` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ExpiredLockerBookings`(lockerTransactionId int)
BEGIN
DECLARE SelectStatement VARCHAR(2000);

SET @SelectStatement = 'Select l.lockerTransactionsId,l.LockerDetailId,l.StoragePeriodStart,l.StoragePeriodEnd,ut.DeviceType,ut.Token,u.Email,ld.LockerNumber
,cl.Description as CabinetLocationDescription,l.IsSubscriptionBooking,l.PaymentReference,l.BookingStatus,l.DropOffCode,l.PickUpCode from locker_bookings l
join locker_detail ld on ld.LockerDetailId=l.LockerDetailId
join cabinets c on ld.CabinetId=c.CabinetId
join cabinet_locations cl ON c.CabinetLocationId = cl.CabinetLocationId
join users u on l.UserKeyId=u.UserKeyId
left join user_token ut on ut.UserId = u.UserId
 where Now()>l.StoragePeriodEnd and (l.BookingStatus=1 or l.BookingStatus=2) 
 and u.IsDeleted=0 and (ut.IsEnable=1 or ut.IsEnable is null) and
 (l.IsNotified=FALSE or l.IsNotified is Null)';

IF lockerTransactionId IS NOT NULL THEN
SET @SelectStatement = CONCAT(@SelectStatement , ' AND l.lockerTransactionsId=', lockerTransactionId);
END IF;

PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAllFeedbacks` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetAllFeedbacks`()
BEGIN

Select t1.FeedbackId,t1.AppRating,t1.FeaturesExpectations,t1.LockerEquipmentRating,t1.Suggestion,
t1.LocationRating,t1.WantToSee,t1.BetterExperience,t1.DateCreated,t1.DateModified,t1.UserKeyId,
t2.Firstname,t2.Lastname
From feedback t1 
Inner Join users t2
On t1.UserKeyId = t2.UserKeyId
Where t1.IsDeleted = 0; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAllFranchiseFeedbackAnswer` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetAllFranchiseFeedbackAnswer`()
BEGIN
select t1.Id,t1.Answer,t1.DateCreated,t1.DateModified,t1.IsDeleted,t1.CompanyId,t2.BusinessName,t2.CompanyName,t3.Question,t3.Type
from franchise_feedback_answer t1
inner join company t2 on t1.CompanyId = t2.CompanyId
inner join franchise_feedback_question t3 on t1.QuestionId = t3.Id
Where t1.IsDeleted = 0;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAllFranchiseFeedbackQuestion` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetAllFranchiseFeedbackQuestion`()
BEGIN
select Id,Question,DateCreated,DateModified,IsDeleted,Type
from franchise_feedback_question;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetAvailableLockers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetAvailableLockers`(
cabinetLocationId int,
    lockerTypeId int,
    selectedMonth int,
    selectedYear int,
    startDate datetime,
    endDate datetime,
    isOrderByLockerNumber tinyint,
	cabinetId int,
    currentPage int,
    pageSize int,
     positionId int
)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(1000);
DECLARE v_offset int;

SET @SelectStatement = ' SELECT
 Count(*) Over () AS TotalRecordCount,
v.CabinetLocationDescription,  
v.Address,
v.Latitude,
v.Longitude,
v.Size,
                            v.LockerTypeDescription,
v.CabinetId,
v.CabinetLocationId,
v.LockerDetailId,
v.LockerTypeId,
v.LockerNumber,

                            v.OpenCommand,
v.GetStatusCommand,
                            (Select o.overstaycharge from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as OverstayCharge,
(Select p.overstayperiod from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as OverstayPeriod,
(Select pt.Name from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as PricingType,
(Select o.storageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as StoragePrice,
(Select o.multiaccessstorageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as MultiAccessStoragePrice,
v.PositionId                             
FROM vw_active_locker AS v ';
SET @WhereClause = '';

IF cabinetLocationId IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.CabinetLocationId = ', cabinetLocationId);
END IF;

IF positionId IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.PositionId = ', positionId);
END IF;

IF cabinetId IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.CabinetId = ', cabinetId);
END IF;

IF lockerTypeId IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.LockerTypeId = ', lockerTypeId);
END IF;

IF selectedMonth IS NOT NULL AND selectedYear IS NOT NULL THEN
 IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND (');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' OR (YEAR(date(b.StoragePeriodStart)) = ', selectedYear, ' OR YEAR(date(b.StoragePeriodEnd)) = ', selectedYear ,')' );
    SET @WhereClause = CONCAT(@WhereClause , ' OR (MONTH(date(b.StoragePeriodStart)) = ', selectedMonth, ' OR MONTH(date(b.StoragePeriodEnd)) = ', selectedMonth ,'))' );

END IF;
IF startDate IS NOT NULL AND endDate IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' AND v.LockerDetailId NOT IN ( SELECT d.LockerDetailId FROM vw_active_locker AS d LEFT JOIN locker_bookings b ON b.LockerDetailId =d.LockerDetailId');
    SET @WhereClause = CONCAT(@WhereClause , " WHERE (");
    SET @WhereClause = CONCAT(@WhereClause ,"(", "'",startDate,"'" , " > b.StoragePeriodStart AND ", "'",startDate,"'", "< b.StoragePeriodEnd",")");
    SET @WhereClause = CONCAT(@WhereClause , " OR (", "'",endDate,"'" , " > b.StoragePeriodStart AND ", "'",endDate,"'", "< b.StoragePeriodEnd",")");
	 SET @WhereClause = CONCAT(@WhereClause ," OR ","'", startDate,"'"," = b.StoragePeriodStart");
    SET @WhereClause = CONCAT(@WhereClause , " OR ","'",  endDate,"'"," = b.StoragePeriodEnd");
SET @WhereClause = CONCAT(@WhereClause ,  '))');
END IF;

IF CHAR_LENGTH(@WhereClause) > 0 THEN
SET @SelectStatement =  CONCAT(@SelectStatement,' WHERE ', @WhereClause);
END IF;

IF isOrderByLockerNumber = 1 THEN
SET @SelectStatement = CONCAT(@SelectStatement , ' ORDER BY v.LockerDetailId ASC');
END IF;

IF pageSize IS NOT NULL AND currentPage IS NOT NULL
THEN
SET @v_offset = (currentPage - 1)* pageSize;
    set @SelectStatement = CONCAT(@SelectStatement  , ' LIMIT ',@v_offset,',',PageSize);
END IF;




PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetBookingsCount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetBookingsCount`(companyId int)
BEGIN
IF companyId IS NOT NULL AND companyId != 0 THEN
SELECT  COUNT(b.LockerTransactionsId) AS bookings FROM locker_bookings b
                     JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
                     JOIN cabinets c ON c.CabinetId = d.CabinetId
                     JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
					 Where l.CompanyId = companyId and PaymentReference IS NOT NULL;
                    
ELSE
Select COUNT(*) AS bookings From locker_bookings where PaymentReference IS NOT NULL;
END IF; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetBookingTransactions` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetBookingTransactions`(
companyId int,
startDate datetime,
endDate datetime,
currentPage int,
pageSize int,
bookingStatus int,
userKeyId varchar(100)

)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(1000);
DECLARE v_offset int;
SET @SelectStatement = 'SELECT  
Count(*) Over () AS TotalRecordCount,
b.LockerTransactionsId,
b.LockerTransactionTypeId,
b.UserKeyId,
b.LockerDetailId,
b.StoragePeriodStart,
b.StoragePeriodEnd,
b.PackageImage,
b.SenderName,
b.SenderMobile,
b.SenderEmailAddress,
b.DropOffDate,
b.DropOffCode,
b.DropOffQRCode,
b.PickupDate,
b.PickUpCode,
b.PickUpQRCode,
b.ReceiverName,
b.ReceiverEmailAddress,
b.ReceiverPhoneNumber,
b.TotalPrice,
b.PaymentMethodId,
b.PaymentReference,
b.BookingStatus,
b.DateCreated,
b.DateModified,
u.Email as UserEmail,
u.Firstname as UserFirstName,
u.Lastname as UserLastName,
u.Photo as UserPhoto,
cl.Description as LocationDescription,
cl.Address as LocationAdress,
ca.CabinetLocationId as CabinetLocationId,
co.FirstName as CompanyFirstName,
co.MiddleName as CompanyMiddleName,
co.BusinessName as CompanyBusinessName,
co.Email as CompanyEmail,
co.CompanyId as CompanyId,
pm.Name as PaymentMethodName,
pm.Description as PaymentMethodDescription,
ld.LockerTypeId as LockerTypeId,
ld.PositionId as PositionId,
ld.LockerNumber as LockerNumber

FROM locker_bookings b 
LEFT JOIN locker_detail ld ON ld.LockerDetailId = b.LockerDetailId
LEFT JOIN cabinets ca ON ca.CabinetId = ld.CabinetId
LEFT JOIN cabinet_locations cl ON cl.CabinetLocationId = ca.CabinetLocationId 
LEFT JOIN users u ON u.UserKeyId = b.UserKeyId
LEFT JOIN company co ON co.CompanyId = cl.CompanyId
LEFT JOIN payment_methods pm ON pm.PaymentMethodId = b.PaymentMethodId';

SET @WhereClause = '';

IF companyId IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' AND cl.CompanyId = ', companyId);
END IF;

IF startDate IS NOT NULL AND endDate IS NOT NULL THEN
     
    SET @WhereClause = CONCAT(@WhereClause , ' AND ',"'",DATE(startDate),"'" , ' <= DATE(b.StoragePeriodStart) AND ', "'",DATE(endDate),"'", '>= DATE(b.StoragePeriodEnd)');
 
END IF;

IF bookingStatus IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' AND b.BookingStatus = ', bookingStatus);
END IF;

IF userKeyId IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' AND b.UserKeyId= ', userKeyId);
END IF;

IF CHAR_LENGTH(@WhereClause) > 0 THEN
SET @SelectStatement = CONCAT(@SelectStatement, ' WHERE ', RIGHT(@WhereClause, CHAR_LENGTH(@WhereClause)-4));
END IF;



IF pageSize IS NOT NULL AND currentPage IS NOT NULL
THEN
SET @v_offset = (currentPage - 1)* pageSize;
    set @SelectStatement = CONCAT(@SelectStatement  , ' LIMIT ',@v_offset,',',PageSize);
END IF;
PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetBookingUpdatedPrice` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetBookingUpdatedPrice`(lockerDetailId int,lockerTransactionsId int,endDate datetime)
BEGIN
SELECT b.LockerDetailId,
b.LockerTransactionsId,
b.StoragePeriodStart,
b.StoragePeriodEnd,
(Select o.overstaycharge from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=c.cabinetlocationid and o.lockertypeid=l.lockertypeid) as OverstayCharge,
(Select p.overstayperiod from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=c.cabinetlocationid and o.lockertypeid=l.lockertypeid) as OverstayPeriod,
(Select pt.Name from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=c.cabinetlocationid and o.lockertypeid=l.lockertypeid) as PricingType,
(Select o.storageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=c.cabinetlocationid and o.lockertypeid=l.lockertypeid) as StoragePrice,
(Select o.multiaccessstorageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=c.cabinetlocationid and o.lockertypeid=l.lockertypeid) as MultiAccessStoragePrice  

FROM locker_bookings b inner join locker_detail l on b.LockerDetailId=l.LockerDetailId
inner join cabinets c on c.CabinetId=l.CabinetId
inner join cabinet_locations cl on cl.cabinetLocationId=c.CabinetLocationId
 WHERE (b.BookingStatus=1 or b.BookingStatus=2) 
and b.lockerDetailId=lockerDetailId and b.lockerTransactionsId=lockerTransactionsId
AND b.lockerDetailId NOT IN ( SELECT d.lockerDetailId FROM vw_active_locker AS d 
LEFT JOIN locker_bookings lb ON lb.LockerDetailId =d.LockerDetailId
 WHERE (lb.LockerTransactionsId is null or lb.LockerTransactionsId<>b.LockerTransactionsId) And 
((b.StoragePeriodEnd > lb.StoragePeriodStart AND b.StoragePeriodEnd< lb.StoragePeriodEnd)
OR (endDate > lb.StoragePeriodStart AND endDate < lb.StoragePeriodEnd)
OR b.StoragePeriodEnd = lb.StoragePeriodStart
OR endDate = lb.StoragePeriodEnd
));
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetCleanlinessReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetCleanlinessReport`(
  setMonth int,
  setYear int, 
  companyId int,
  statusId int
)
BEGIN

DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(2000);
 

SET @SelectStatement = "SELECT * FROM vw_cleanliness_report cr ";
SET @WhereClause = '';

IF setMonth > 0 AND setMonth IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause ,  ' AND (cr.Month = ', setMonth, ' OR cr.Month IS NULL',')');
END IF;

IF setYear > 0 AND setYear IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' AND (YEAR(cr.DateSubmitted) = ', setYear, ' OR cr.DateSubmitted IS NULL',')');
END IF;

IF companyId > 0 AND companyId IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause, ' AND cr.CompanyId = ', companyId);
END IF;


IF statusId IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause, ' AND cr.Status = ', statusId);
END IF;

IF CHAR_LENGTH(@WhereClause) > 0 THEN
SET @SelectStatement = CONCAT(@SelectStatement, ' WHERE ', RIGHT(@WhereClause, CHAR_LENGTH(@WhereClause)-4));
END IF;

 
 



PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery; 
DEALLOCATE PREPARE dynquery;
 
 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetDeliveriesCount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetDeliveriesCount`(companyId int)
BEGIN
IF companyId IS NOT NULL AND companyId != 0 THEN
Select COUNT(b.LockerTransactionsId) AS deliveries From locker_bookings b
					 JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
                     JOIN cabinets c ON c.CabinetId = d.CabinetId
                     JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
					 Where b.CompanyId = companyId AND BookingStatus=4;
           
ELSE
Select COUNT(*) AS deliveries From locker_bookings WHERE BookingStatus=4;
END IF; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetDropOffCount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetDropOffCount`(companyId int)
BEGIN
		IF companyId IS NOT NULL AND companyId != 0 THEN
			SELECT  COUNT(b.LockerTransactionsId) AS dropoff FROM locker_bookings b
								 JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
								 JOIN cabinets c ON c.CabinetId = d.CabinetId
								 JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId  Where b.CompanyId = companyId AND bookingstatus = 2;
		ELSE
			Select COUNT(*) AS bookings From locker_bookings  where bookingstatus = 2;
		END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetLocationsCount` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetLocationsCount`(companyId int)
BEGIN
IF companyId IS NOT NULL AND companyId != 0 THEN
Select COUNT(l.CabinetLocationId) AS locations From cabinet_locations l where l.CompanyId = companyId
 And l.isdeleted=0;
                    
ELSE
Select COUNT(*) AS locations From cabinet_locations  Where  isdeleted=0 ;
END IF; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetLogs` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetLogs`(
  startDate datetime,
  endDate datetime, 
  logType varchar(20),
  search varchar(200),
  currentPage int,
    pageSize int
)
BEGIN

DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(2000);
 

SET @SelectStatement = "SELECT *, Count(*) Over () AS TotalRecordCount  FROM nlogs";
SET @WhereClause = '';

IF startDate IS NOT NULL THEN
 SET @WhereClause = CONCAT(@WhereClause , ' AND', "'" , DATE(startDate), "'" , ' <= DATE(Logged)'); 
END IF;

IF endDate IS NOT NULL THEN
 SET @WhereClause = CONCAT(@WhereClause , ' AND', "'" , DATE(endDate), "'" , ' >= DATE(Logged)'); 
END IF;

 IF logType IS NOT NULL THEN
 SET @WhereClause = CONCAT(@WhereClause ,  ' AND Level = ', "'" , logType, "'" );
 END IF;
  
 IF search IS NOT NULL THEN
 SET @WhereClause = CONCAT(@WhereClause ,  ' AND Message LIKE ',"'%" , search, "%'");
 END IF;
 
 IF CHAR_LENGTH(@WhereClause) > 0 THEN
 SET @SelectStatement = CONCAT(@SelectStatement, ' WHERE ', RIGHT(@WhereClause, CHAR_LENGTH(@WhereClause)-4));
 END IF;
 
SET @v_offset = (currentPage - 1)* PageSize;
set @SelectStatement = CONCAT(@SelectStatement  , 'ORDER BY ID DESC LIMIT ',@v_offset,',',PageSize);
 
 



PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery; 
DEALLOCATE PREPARE dynquery; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetMessageLog` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetMessageLog`(companyId int,currentPage int,pageSize int)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(1000);
DECLARE v_offset int;
SET @SelectStatement = 'Select Count(Id) Over () AS TotalRecordCount,Id,isSent,Subject,Message,Receipent,Sender,DateCreated,CompanyId,Type from message_log';
SET @WhereClause = '';
IF companyId IS NOT NULL THEN
SET @WhereClause = CONCAT(@WhereClause , ' CompanyId=', companyId);
END IF;
IF CHAR_LENGTH(@WhereClause) > 0 THEN
SET @SelectStatement = CONCAT(@SelectStatement, ' WHERE ', @WhereClause);
END IF;
IF pageSize IS NOT NULL AND currentPage IS NOT NULL
THEN
SET @v_offset = (currentPage - 1)* pageSize;
    set @SelectStatement = CONCAT(@SelectStatement  , ' LIMIT ',@v_offset,',',PageSize);
END IF;
PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetMostBookedLockers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetMostBookedLockers`(companyId int)
BEGIN
DECLARE WhereClause VARCHAR(1000);
 
IF companyId IS NOT NULL THEN 
 SELECT d.LockerDetailId ,l.Description, lt.Size , (ROUND(COUNT(d.LockerDetailId)/(SELECT COUNT(*) FROM locker_bookings b
        JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
        JOIN cabinets c ON c.CabinetId = d.CabinetId
        JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
        JOIN locker_types lt ON lt.LockerTypeId = d.LockerTypeId
        WHERE b.CompanyId = 1)*100)) as Percentage
 
		FROM locker_bookings b
        JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
        JOIN cabinets c ON c.CabinetId = d.CabinetId
        JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
        JOIN locker_types lt ON lt.LockerTypeId = d.LockerTypeId
        WHERE b.CompanyId = companyId GROUP BY d.LockerDetailId Order By Percentage DESC LIMIT 7;
ELSE
SELECT d.LockerDetailId ,l.Description, lt.Size , (ROUND(COUNT(d.LockerDetailId)/(SELECT COUNT(*) FROM locker_bookings b
        JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
        JOIN cabinets c ON c.CabinetId = d.CabinetId
        JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
        JOIN locker_types lt ON lt.LockerTypeId = d.LockerTypeId)*100)) as Percentage
 
		FROM locker_bookings b
        JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
        JOIN cabinets c ON c.CabinetId = d.CabinetId
        JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
        JOIN locker_types lt ON lt.LockerTypeId = d.LockerTypeId
		GROUP BY d.LockerDetailId Order By Percentage  DESC LIMIT 7;
END IF;
 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetRecentBookings` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetRecentBookings`(companyId int)
BEGIN
IF companyId IS NOT NULL AND companyId != 0 THEN
SELECT l.Description, t.Size ,b.LockerTransactionsId, b.DateCreated FROM locker_bookings b
                     JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
                     JOIN cabinets c ON c.CabinetId = d.CabinetId
                     JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
                    JOIN locker_types t ON t.LockerTypeId = d.LockerTypeId
					Where l.CompanyId = companyId  order by b.DateCreated desc limit 10;
                    
ELSE 
SELECT l.Description, t.Size ,b.LockerTransactionsId, b.DateCreated FROM locker_bookings b
                     JOIN locker_detail d ON d.LockerDetailId = b.LockerDetailId
                     JOIN cabinets c ON c.CabinetId = d.CabinetId
                     JOIN cabinet_locations l ON l.CabinetLocationId = c.CabinetLocationId
                    JOIN locker_types t ON t.LockerTypeId = d.LockerTypeId
					 order by b.DateCreated desc limit 10;
END IF; 
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetRevenue` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetRevenue`(companyId int)
BEGIN
IF companyId IS NOT NULL THEN
SELECT MONTHNAME(B.DateCreated) as Month, SUM(B.TotalPrice) as Revenue
FROM locker_bookings B Where Year(B.DateCreated) = Year(current_date()) AND B.CompanyId = companyId
GROUP BY YEAR(B.DateCreated),MONTH(B.DateCreated)
ORDER BY MONTH(DateCreated);
ELSE
SELECT MONTHNAME(B.DateCreated) as Month, SUM(B.TotalPrice) as Revenue
FROM locker_bookings B Where Year(B.DateCreated) = Year(current_date())
GROUP BY YEAR(B.DateCreated),MONTH(B.DateCreated)
ORDER BY MONTH(B.DateCreated);
END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetTodayNotifications` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetTodayNotifications`(companyId int)
BEGIN
IF companyId IS NOT NULL THEN
 
SELECT Description , DateCreated
FROM notifications Where CabinetLocationId 
in((SELECT CabinetLocationId FROM company_cabinet Where company_cabinet.CompanyId = companyId)) AND DAY(DateCreated) = DAY(current_date()) 

Order By DateCreated desc LIMIT 5;
END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetUpdatedAvailableLockers` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetUpdatedAvailableLockers`(cabinetLocationId int,
    lockerTypeId int,
    startDate datetime,
    endDate datetime,
    isOrderByLockerNumber tinyint,
	cabinetId int,
    currentPage int,
    pageSize int,
    positionId int)
BEGIN
DECLARE SelectStatement VARCHAR(2000);
DECLARE WhereClause VARCHAR(1000);
DECLARE v_offset int;

SET @SelectStatement = ' SELECT
Distinct Count(*) Over () AS TotalRecordCount,
v.CabinetLocationDescription,  
v.Address,
v.Latitude,
v.Longitude,
v.Size,
                            v.LockerTypeDescription,
v.CabinetId,
v.CabinetLocationId,
v.LockerDetailId,
v.LockerTypeId,
v.LockerNumber,

                            v.OpenCommand,
v.GetStatusCommand,
                            (Select o.overstaycharge from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as OverstayCharge,
(Select p.overstayperiod from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as OverstayPeriod,
(Select pt.Name from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as PricingType,
(Select o.storageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as StoragePrice,
(Select o.multiaccessstorageprice from overstay_price_configuration o 
inner join pricing_matrix p on p.id = o.pricingmatrixid 
inner join pricing_type pt on p.pricingtypeid =pt.id where 
o.locationid=v.cabinetlocationid and o.lockertypeid=v.lockertypeid) as  MultiAccessStoragePrice,
v.PositionId                        
FROM vw_active_locker AS v ';
SET @WhereClause = '';
IF cabinetLocationId IS NOT NULL THEN
  IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.CabinetLocationId = ', cabinetLocationId);
END IF;

IF positionId IS NOT NULL THEN
IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.PositionId = ', positionId);
END IF;

IF cabinetId IS NOT NULL THEN
IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
  END IF;
SET @WhereClause = CONCAT(@WhereClause , ' v.CabinetId = ', cabinetId);
END IF;

IF lockerTypeId IS NOT NULL THEN
   IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
   END IF;
SET @WhereClause = CONCAT(@WhereClause , '  v.LockerTypeId = ', lockerTypeId);
END IF;
IF startDate IS NOT NULL AND endDate IS NOT NULL THEN
   IF(CHAR_LENGTH(@WhereClause) > 0) THEN
	 SET @WhereClause = CONCAT(@WhereClause , ' AND ');
   END IF;
SET @WhereClause = CONCAT(@WhereClause , " v.LockerDetailId NOT IN ( SELECT d.LockerDetailId FROM vw_active_locker AS d LEFT JOIN locker_bookings b ON b.LockerDetailId =d.LockerDetailId");
    SET @WhereClause = CONCAT(@WhereClause , " WHERE (");
    SET @WhereClause = CONCAT(@WhereClause ,"(", "'",startDate,"'" , " > b.StoragePeriodStart AND ", "'",startDate,"'", "< b.StoragePeriodEnd",")");
    SET @WhereClause = CONCAT(@WhereClause , " OR (", "'",endDate,"'" , " > b.StoragePeriodStart AND ", "'",endDate,"'", "< b.StoragePeriodEnd",")");
	 SET @WhereClause = CONCAT(@WhereClause ," OR ","'", startDate,"'"," = b.StoragePeriodStart");
    SET @WhereClause = CONCAT(@WhereClause , " OR ","'",  endDate,"'"," = b.StoragePeriodEnd");
SET @WhereClause = CONCAT(@WhereClause ,  "))");
END IF;


IF CHAR_LENGTH(@WhereClause) > 0 THEN
SET @SelectStatement = CONCAT(@SelectStatement,' WHERE ', @WhereClause);
END IF;

IF isOrderByLockerNumber = 1 THEN
SET @SelectStatement = CONCAT(@SelectStatement , ' ORDER BY v.LockerDetailId ASC');
END IF;

IF pageSize IS NOT NULL AND currentPage IS NOT NULL
THEN
SET @v_offset = (currentPage - 1)* pageSize;
    set @SelectStatement = CONCAT(@SelectStatement  , ' LIMIT ',@v_offset,',',PageSize);
END IF;




PREPARE dynquery FROM @SelectStatement;
EXECUTE dynquery;
DEALLOCATE PREPARE dynquery;


END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_GetYesterdayNotifications` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_GetYesterdayNotifications`(companyId int)
BEGIN
IF companyId IS NOT NULL THEN
 
SELECT Description , DateCreated
FROM notifications Where CabinetLocationId 
in((SELECT CabinetLocationId FROM company_cabinet Where company_cabinet.CompanyId = companyId)) AND DAY(DateCreated) =  DAY(current_date() -1) 

Order By DateCreated desc LIMIT 5;
END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_LockerBookingDetail` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_LockerBookingDetail`(lockerTransactionId int)
BEGIN
Select l.lockerTransactionsId,l.LockerDetailId,l.StoragePeriodStart,l.StoragePeriodEnd,ld.LockerNumber
,cl.Description as CabinetLocationDescription,u.Email as UserEmail,u.FirstName as UserFirstName,l.DropOffCode,l.PickUpCode,l.PaymentReference
 from locker_bookings l
join locker_detail ld on ld.LockerDetailId=l.LockerDetailId
join cabinets c on ld.CabinetId=c.CabinetId
join cabinet_locations cl ON c.CabinetLocationId = cl.CabinetLocationId
join users u on l.UserKeyId=u.UserKeyId
 and l.lockerTransactionsId=lockerTransactionId;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_LogEntry` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_LogEntry`(
 
 IN machineName varchar(200),
  IN logged datetime,
 IN level varchar(5),
 IN message longtext,
 IN logger varchar(300),
 IN properties longtext,
 IN callsite varchar(300),
IN  exception longtext
)
BEGIN
 INSERT INTO nlogs (
	 
    MachineName,
    Logged,
    Level,
    Message,
    Logger,
    Properties,
    Callsite,
    Exception
  ) VALUES (
 
    machineName,
    logged,
    level,
    message,
    logger,
    properties,
    callsite,
    exception
  );
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_UnsubmittedCleanlinessReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_UnsubmittedCleanlinessReport`(month int)
BEGIN
Select companyid,companyname,contactnumber,contactperson,email,mobilenumber 
from company where isdeleted=0 and companyid not in(select companyid from cleanliness_report 
where Month=month and ((status=1 or status=2) or IsSubmitted=true));
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_UserAnuallyReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_UserAnuallyReport`(userKey varchar(16),selectedYear int)
BEGIN
Select count(LockerTransactionsId) as TotalBookings,month(DateCreated) as Month from locker_bookings
where UserKeyId=userKey and year(DateCreated)=selectedYear group by month(DateCreated);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_UserMonthlyReport` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_UserMonthlyReport`(userKey varchar(16),selectedMonth int,status int)
BEGIN
Select count(LockerTransactionsId) from locker_bookings where (UserKeyId=userKey and month(DateCreated)=selectedMonth and Bookingstatus=status);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_ValidateBookingTransaction` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `sp_ValidateBookingTransaction`(companyid int,lockertransactionid int)
BEGIN
Select count(lockertransactionsid) as totalcount FROM locker_bookings b 
    INNER JOIN locker_detail ld ON ld.LockerDetailId = b.LockerDetailId
    INNER JOIN cabinets ca ON ca.CabinetId = ld.CabinetId
	INNER JOIN cabinet_locations cl ON cl.CabinetLocationId = ca.CabinetLocationId 
    where cl.CompanyId=companyid and b.lockertransactionsid=lockertransactionid;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `test-pager` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`admin`@`%` PROCEDURE `test-pager`(
   _PageIndex int,
   _PageSize int,
   OUT _RecordCount int
)
BEGIN
  SET @RowNumber:=0;
 
       CREATE TEMPORARY TABLE Results
       SELECT @RowNumber:=@RowNumber+1 RowNumber
			  ,LockerDetailId
              ,CabinetId
              ,LockerTypeId
              ,LockerNumber
       FROM Customers;
 
       SET _RecordCount =(SELECT COUNT(*) FROM Results);
 
       SELECT * FROM Results
       WHERE RowNumber BETWEEN(_PageIndex -1) * _PageSize + 1 AND(((_PageIndex -1) * _PageSize + 1) + _PageSize) - 1;
 
       DROP TEMPORARY TABLE Results;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `test_pager` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `test_pager`(
   _PageIndex int,
   _PageSize int,
	OUT _Recordcount int,
	IN Param1 VARCHAR(255)
)
BEGIN
  DECLARE SelectStatement VARCHAR(2000);
  SET @RowNumber:=0;
  SET @RecordCount:=0;
  SET @Test=Param1;
  
      
       SET @SelectStatement = 'SELECT @RowNumber:=@RowNumber+1 RowNumber
			  ,LockerDetailId
              ,CabinetId
              ,LockerTypeId
              ,LockerNumber
       FROM smartbox.locker_detail';
 
		PREPARE dynquery FROM @SelectStatement;
		EXECUTE dynquery USING @Test;
		DEALLOCATE PREPARE dynquery;
        
        SELECT @Test;
      
 
      
       
 
       
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `test_pager2` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE  PROCEDURE `test_pager2`(
	IN Param1 VARCHAR(255), 
	OUT Param2 VARCHAR(255), 
	OUT Param3 VARCHAR(255)

)
BEGIN
SET @c2 = '';
  SET @c3 = '';
  SET @s = 'SELECT LockerDetailId, LockerNumber INTO @c2, @c3 FROM smartbox.locker_detail';
  PREPARE stmt FROM @s;
  SET @c1 = Param1;
  EXECUTE stmt USING @c1;
  DEALLOCATE PREPARE stmt;
  SET Param2 = @c2;
  SET Param3 = @c3;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `vw_locker_booking_locker`
--

/*!50001 DROP VIEW IF EXISTS `vw_locker_booking_locker`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013  SQL SECURITY DEFINER */
/*!50001 VIEW `vw_locker_booking_locker` AS select `b`.`LockerTransactionsId` AS `LockerTransactionsId`,`b`.`UserKeyId` AS `UserKeyId`,`b`.`CompanyId` AS `CompanyId`,`b`.`LockerDetailId` AS `LockerDetailId`,`b`.`StoragePeriodStart` AS `StoragePeriodStart`,`b`.`StoragePeriodEnd` AS `StoragePeriodEnd`,`b`.`NewStoragePeriodEndDate` AS `NewStoragePeriodEndDate`,`b`.`SenderName` AS `SenderName`,`b`.`SenderMobile` AS `SenderMobile`,`b`.`SenderEmailAddress` AS `SenderEmailAddress`,`b`.`PackageImage` AS `PackageImage`,`b`.`DropOffDate` AS `DropOffDate`,`b`.`DropOffCode` AS `DropOffCode`,`b`.`DropOffQRCode` AS `DropOffQRCode`,`b`.`PickupDate` AS `PickupDate`,`b`.`PickUpCode` AS `PickUpCode`,`b`.`PickUpQRCode` AS `PickUpQRCode`,`b`.`ReceiverName` AS `ReceiverName`,`b`.`ReceiverEmailAddress` AS `ReceiverEmailAddress`,`b`.`ReceiverPhoneNumber` AS `ReceiverPhoneNumber`,`b`.`TotalPrice` AS `TotalPrice`,`b`.`PaymentMethodId` AS `PaymentMethodId`,`b`.`PaymentReference` AS `PaymentReference`,`b`.`BookingStatus` AS `BookingStatus`,`b`.`AccessPlan` AS `AccessPlan`,`b`.`DateCreated` AS `DateCreated`,`b`.`DateModified` AS `DateModified`,`d`.`LockerNumber` AS `LockerNumber`,`d`.`BoardNumber` AS `BoardNumber`,`d`.`OpenCommand` AS `OpenCommand`,`d`.`GetStatusCommand` AS `GetStatusCommand`,`b`.`IsSubscriptionBooking` AS `IsSubscriptionBooking`,`d`.`CabinetLocationDescription` AS `CabinetLocationDescription` from (`locker_bookings` `b` join `vw_active_locker` `d` on((`b`.`LockerDetailId` = `d`.`LockerDetailId`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-07-03 14:15:58
