import { Result } from 'antd'
import { useTranslation } from 'react-i18next'

interface Props {
  labelKey: string
}

export default function ComingSoon({ labelKey }: Props) {
  const { t } = useTranslation()
  return (
    <Result
      status="info"
      title={t(labelKey)}
      subTitle={t('common.comingSoon')}
    />
  )
}
