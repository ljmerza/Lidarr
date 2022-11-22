import PropTypes from 'prop-types';
import React from 'react';
import Alert from 'Components/Alert';
import Form from 'Components/Form/Form';
import FormGroup from 'Components/Form/FormGroup';
import FormInputGroup from 'Components/Form/FormInputGroup';
import FormLabel from 'Components/Form/FormLabel';
import Button from 'Components/Link/Button';
import SpinnerErrorButton from 'Components/Link/SpinnerErrorButton';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import ModalBody from 'Components/Modal/ModalBody';
import ModalContent from 'Components/Modal/ModalContent';
import ModalFooter from 'Components/Modal/ModalFooter';
import ModalHeader from 'Components/Modal/ModalHeader';
import { inputTypes, kinds, sizes } from 'Helpers/Props';
import { numberSettingShape, stringSettingShape, tagSettingShape } from 'Helpers/Props/Shapes/settingShape';
import translate from 'Utilities/String/translate';
import DownloadProtocolItems from './DownloadProtocolItems';
import styles from './EditDelayProfileModalContent.css';

function EditDelayProfileModalContent(props) {
  const {
    id,
    isFetching,
    isPopulated,
    error,
    isSaving,
    saveError,
    item,
    onInputChange,
    onSavePress,
    onModalClose,
    onDeleteDelayProfilePress,
    ...otherProps
  } = props;

  const {
    name,
    items,
    tags
  } = item;

  return (
    <ModalContent onModalClose={onModalClose}>
      <ModalHeader>
        {id ? translate('EditDelayProfile') : translate('AddDelayProfile')}
      </ModalHeader>

      <ModalBody>
        {
          isFetching ?
            <LoadingIndicator /> :
            null
        }

        {
          !isFetching && !!error ?
            <div>
              {translate('UnableToAddANewDelayProfilePleaseTryAgain')}
            </div> :
            null
        }

        {
          !isFetching && isPopulated && !error ?
            <Form {...otherProps}>
              <FormGroup size={sizes.SMALL}>
                <FormLabel size={sizes.SMALL}>
                  {translate('Name')}
                </FormLabel>

                <FormInputGroup
                  type={inputTypes.TEXT}
                  name="name"
                  {...name}
                  onChange={onInputChange}
                />
              </FormGroup>

              <div className={styles.formGroupWrapper}>
                <DownloadProtocolItems
                  items={items.value}
                  errors={items.errors}
                  warnings={items.warnings}
                  {...otherProps}
                />
              </div>

              {
                id === 1 ?
                  <Alert>
                    This is the default profile. It applies to all artists that don't have an explicit profile.
                  </Alert> :

                  <FormGroup size={sizes.SMALL}>
                    <FormLabel size={sizes.SMALL}>
                      {translate('Tags')}
                    </FormLabel>

                    <FormInputGroup
                      type={inputTypes.TAG}
                      name="tags"
                      {...tags}
                      helpText={translate('TagsHelpText')}
                      onChange={onInputChange}
                    />
                  </FormGroup>
              }
            </Form> :
            null
        }
      </ModalBody>
      <ModalFooter>
        {
          id && id > 1 ?
            <Button
              className={styles.deleteButton}
              kind={kinds.DANGER}
              onPress={onDeleteDelayProfilePress}
            >
              {translate('Delete')}
            </Button> :
            null
        }

        <Button
          onPress={onModalClose}
        >
          {translate('Cancel')}
        </Button>

        <SpinnerErrorButton
          isSpinning={isSaving}
          error={saveError}
          onPress={onSavePress}
        >
          {translate('Save')}
        </SpinnerErrorButton>
      </ModalFooter>
    </ModalContent>
  );
}

const delayProfileShape = {
  name: PropTypes.shape(stringSettingShape).isRequired,
  items: PropTypes.object.isRequired,
  order: PropTypes.shape(numberSettingShape),
  tags: PropTypes.shape(tagSettingShape).isRequired
};

EditDelayProfileModalContent.propTypes = {
  id: PropTypes.number,
  isFetching: PropTypes.bool.isRequired,
  isPopulated: PropTypes.bool.isRequired,
  error: PropTypes.object,
  isSaving: PropTypes.bool.isRequired,
  saveError: PropTypes.object,
  item: PropTypes.shape(delayProfileShape).isRequired,
  onInputChange: PropTypes.func.isRequired,
  onSavePress: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired,
  onDeleteDelayProfilePress: PropTypes.func
};

export default EditDelayProfileModalContent;
