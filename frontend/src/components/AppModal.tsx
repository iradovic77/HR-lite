import { Modal, type ModalProps } from 'antd'

export default function AppModal(props: ModalProps) {
  return <Modal maskClosable={false} {...props} />
}
