CREATE OR REPLACE PROCEDURE public.sp_iheaudit_insert(
	IN vihe_event_id integer,
	IN vihe_event_type integer,
	IN vihe_action_code character,
	IN vihe_syslog_severity integer,
	IN vihe_xml_message bytea,
	IN vacuo_event_type character varying,
	IN vacuo_user character varying DEFAULT 'N/A'::character varying,
	IN vacuo_pt_id character varying DEFAULT 'N/A'::character varying,
	IN vacuo_pt_name character varying DEFAULT 'N/A'::character varying,
	IN vacuo_st_dicom_uid character varying DEFAULT 'N/A'::character varying,
	IN vacuo_se_modality character varying DEFAULT 'N/A'::character varying,
	IN vacuo_se_dicom_uid character varying DEFAULT 'N/A'::character varying,
	IN vacuo_im_sopinstance_uid character varying DEFAULT 'N/A'::character varying,
	IN vacuo_file_name character varying DEFAULT 'N/A'::character varying,
	IN vacuo_source character varying DEFAULT 'N/A'::character varying,
	IN vacuo_machine character varying DEFAULT 'N/A'::character varying,
	IN vacuo_database character varying DEFAULT 'N/A'::character varying,
	IN vacuo_comments character varying DEFAULT 'N/A'::character varying,
	IN vacuo_accession_number character varying DEFAULT 'N/A'::character varying,
	IN vacuo_acuostore_server character varying DEFAULT NULL::character varying,
	IN vacuo_acuostore_appl character varying DEFAULT NULL::character varying,
	IN vacuo_acuostore_username character varying DEFAULT NULL::character varying,
	IN vacuo_acuostore_password character varying DEFAULT NULL::character varying,
	IN vacuo_recycle_bin_folder bytea DEFAULT NULL::bytea,
	IN vacuo_image_guid bytea DEFAULT NULL::bytea,
	IN val_deletion_set_id character varying DEFAULT NULL::character varying,
	IN val_query_string text DEFAULT NULL::text,
	IN val_audit_level smallint DEFAULT NULL::smallint,
	IN val_audit_action smallint DEFAULT NULL::smallint)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE 
	v_auditd uuid;
	v_LogDate timestamp(3);
	v_LogDateUtc timestamp(3);
	v_CharPos int;
	v_Domain varchar(65);
	v_User varchar(65);
BEGIN
	SELECT gen_random_uuid() INTO v_auditd;
	SELECT now() INTO v_LogDate;
	SELECT now() AT time zone 'utc' INTO v_LogDateUtc;
	
	-- Let's table an IHE XML based audit message first.  We need the id to link the
	-- old Acuo audit table to the new message.

	INSERT INTO public."T_IHE_AUDIT"
	Values
	(
		v_auditd,
		v_LogDate,
		vIHE_SYSLOG_SEVERITY,
		vIHE_EVENT_ID,
		vIHE_EVENT_TYPE,
		vIHE_ACTION_CODE,
		vIHE_XML_MESSAGE,
		false
	);

	-- We also insert the data into our old audit log format, so we can search it
	-- When SQL Server 2005 is available we will use XML search technologies
	-- and try to eliminate this table.  We only table the deletion of images.
	IF  (vIHE_EVENT_ID = 110103 AND (vIHE_ACTION_CODE = 'D' OR vIHE_ACTION_CODE = 'U')) 
	THEN
		INSERT INTO public."T_ACUO_AUDIT"
		Values
		(
			v_LogDate,
			vACUO_EVENT_TYPE,
			vACUO_USER,
			vACUO_PT_ID,
			vACUO_PT_NAME,
			vACUO_ST_DICOM_UID,
			vACUO_SE_MODALITY,
			vACUO_SE_DICOM_UID,
			vACUO_IM_SOPINSTANCE_UID,
			vACUO_FILE_NAME,
			vACUO_SOURCE,
			vACUO_MACHINE,
			vACUO_DATABASE,
			vACUO_COMMENTS,
			v_auditd,
			vACUO_ACCESSION_NUMBER,
			vACUO_ACUOSTORE_SERVER, 
			vACUO_ACUOSTORE_APPL, 
			vACUO_ACUOSTORE_USERNAME, 
			vACUO_ACUOSTORE_PASSWORD, 
			vACUO_RECYCLE_BIN_FOLDER, 
			vACUO_IMAGE_GUID,
			vAL_DELETION_SET_ID,
			0,
			null,
			null,
			null,
			null,
			0
		);
	END IF;
		
	-- We also insert the data into our old audit log format, so we can search it
	-- When SQL Server 2005 is available we will use XML search technologies
	-- and try to eliminate this table.  We only table the deletion of images.
	IF  (vIHE_EVENT_ID = 110103 AND (vIHE_ACTION_CODE = 'R') OR vACUO_EVENT_TYPE = 'User Access') 
	THEN
		-- Split Domain and User.
		SELECT strpos(vACUO_USER, '\') INTO v_CharPos;
		IF (v_CharPos > 0)
		THEN
			SELECT SUBSTRING ( vACUO_USER, 1, v_CharPos - 1 ) INTO v_Domain;
			SELECT SUBSTRING ( vACUO_USER, v_CharPos + 1, LEN( vACUO_USER ) ) INTO v_User;
		END IF;

		INSERT INTO public."T_USER_AUDIT"
		Values
		(
			default,
			v_LogDateUtc,
			vAL_AUDIT_LEVEL,
			vAL_AUDIT_ACTION,
			v_Domain,
			v_User,
			vACUO_PT_ID,
			vACUO_PT_NAME,
			vACUO_ST_DICOM_UID,
			vACUO_ACCESSION_NUMBER,
			vACUO_SE_DICOM_UID,
			vACUO_IM_SOPINSTANCE_UID,
			vAL_QUERY_STRING,
			v_auditd,
			0
		);
	END IF;

	-- Log a Task to Send the Message to all the Reliable SysLog Audit Repositories
	INSERT INTO public."T_TASKS"
	("TASK_TARGET_ID", "TASK_IHE_AUDIT_ID", "TASK_STATUS", "TASK_LAST_ERROR", "TASK_QUEUED_TIME", "TASK_XML_MESSAGE", "TASK_RUN_COUNT")
	SELECT 
		"RS_ID",
		v_auditd,
		1,	
		null,
		v_LogDate,
		vIHE_XML_MESSAGE,
		0
	FROM public."T_SYSLOG";
		
	EXCEPTION
		WHEN others THEN
			ROLLBACK;
			RAISE;
			--IF (@@TRANCOUNT > 0)  ROLLBACK TRANSACTION;
			--EXEC [dbo].[USP_RethrowError] @@PROCID;
			--RETURN (1);	
	
END;
$BODY$;
ALTER PROCEDURE public.sp_iheaudit_insert(integer, integer, character, integer, bytea, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, bytea, bytea, character varying, text, smallint, smallint)
    OWNER TO postgres;
