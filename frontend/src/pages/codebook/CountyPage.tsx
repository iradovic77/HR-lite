import { useEffect, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Button, Tag, Space, Form, Input, Select,
  InputNumber, Switch, App, Tooltip, Checkbox,
} from 'antd'
import AppModal from '@/components/AppModal'
import AgGridWrapper from '@/components/AgGridWrapper'
import { PlusOutlined, EditOutlined, StopOutlined, CheckOutlined, DeleteOutlined } from '@ant-design/icons'
import type { ColDef, ICellRendererParams } from 'ag-grid-community'
import {
  countryApi, countyApi,
  type CountryResponse, type CountyResponse, type CreateCountyRequest,
} from '@/api/codebook'
import CodebookLayout from '@/layouts/CodebookLayout'
import { useTheme } from '@/context/ThemeContext'

interface FormValues {
  code: string
  nameHr: string
  nameEn?: string | null
  countryId?: string | null
  ordinal: number
  isActive: boolean
}

export default function CountyPage() {
  const { t } = useTranslation()
  const { message, modal } = App.useApp()
  const { isDark } = useTheme()

  const [data, setData]                   = useState<CountyResponse[]>([])
  const [countries, setCountries]         = useState<CountryResponse[]>([])
  const [loading, setLoading]             = useState(false)
  const [onlyActive, setOnlyActive]       = useState(true)
  const [countryFilter, setCountryFilter] = useState<string | null>(null)
  const [modalOpen, setModalOpen]         = useState(false)
  const [saving, setSaving]               = useState(false)
  const [editingItem, setEditingItem]     = useState<CountyResponse | null>(null)
  const [form] = Form.useForm<FormValues>()

  const fetchCountries = async () => {
    try {
      const res = await countryApi.getAll(false)
      setCountries(res.data)
    } catch { /* silent */ }
  }

  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await countyApi.getAll(true, countryFilter)
      setData(res.data)
    } catch {
      message.error('Greška pri dohvatu podataka')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { fetchCountries() }, [])
  useEffect(() => { fetchData() }, [countryFilter])

  const filteredData = onlyActive ? data.filter(d => d.isActive) : data

  const openAddModal = () => {
    setEditingItem(null)
    form.resetFields()
    form.setFieldsValue({ isActive: true, ordinal: data.length + 1 })
    setModalOpen(true)
  }

  const openEditModal = (item: CountyResponse) => {
    setEditingItem(item)
    form.setFieldsValue({
      code: item.code,
      nameHr: item.nameHr,
      nameEn: item.nameEn,
      countryId: item.countryId,
      ordinal: item.ordinal,
      isActive: item.isActive,
    })
    setModalOpen(true)
  }

  const handleSave = async () => {
    const values = await form.validateFields()
    setSaving(true)
    try {
      if (editingItem) {
        await countyApi.update(editingItem.id, values)
        message.success('Zapis ažuriran')
      } else {
        await countyApi.create(values as CreateCountyRequest)
        message.success('Zapis dodan')
      }
      setModalOpen(false)
      fetchData()
    } catch {
      message.error('Greška pri spremanju')
    } finally {
      setSaving(false)
    }
  }

  const handleToggleActive = async (item: CountyResponse) => {
    try {
      await countyApi.toggleActive(item.id)
      message.success(item.isActive ? 'Zapis deaktiviran' : 'Zapis aktiviran')
      fetchData()
    } catch {
      message.error('Greška pri promjeni statusa')
    }
  }

  const handleDelete = async (item: CountyResponse) => {
    try {
      await countyApi.deleteById(item.id)
      message.success('Zapis obrisan')
      fetchData()
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status
      message.error(status === 409 ? t('common.error_in_use') : t('common.error_delete'))
    }
  }

  const confirmToggleActive = (item: CountyResponse) => {
    modal.confirm({
      title: t(item.isActive ? 'codebook.county.confirm.deactivate' : 'codebook.county.confirm.activate'),
      onOk: () => handleToggleActive(item),
      okText: t('common.yes'),
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const confirmDelete = (item: CountyResponse) => {
    modal.confirm({
      title: t('codebook.county.confirm.delete'),
      content: t('codebook.county.confirm.delete_description'),
      onOk: () => handleDelete(item),
      okText: t('codebook.county.confirm.delete_ok'),
      okType: 'danger',
      cancelText: t('common.no'),
      centered: true,
    })
  }

  const countryOptions = useMemo(
    () => countries.map(c => ({ value: c.id, label: c.nameHr })),
    [countries]
  )

  const columnDefs = useMemo<ColDef<CountyResponse>[]>(() => [
    {
      field: 'code',
      headerName: t('codebook.county.columns.code'),
      width: 80,
    },
    {
      field: 'nameHr',
      headerName: t('codebook.county.columns.nameHr'),
      flex: 1,
    },
    {
      field: 'nameEn',
      headerName: t('codebook.county.columns.nameEn'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'countryNameHr',
      headerName: t('codebook.county.columns.country'),
      flex: 1,
      valueFormatter: (p) => p.value ?? '—',
    },
    {
      field: 'ordinal',
      headerName: t('codebook.county.columns.ordinal'),
      width: 120,
      sort: 'asc',
    },
    {
      field: 'isActive',
      headerName: t('codebook.county.columns.status'),
      width: 120,
      filter: false,
      cellRenderer: (p: ICellRendererParams<CountyResponse>) =>
        p.value
          ? <Tag color="success">{t('codebook.county.status.active')}</Tag>
          : <Tag color="default">{t('codebook.county.status.inactive')}</Tag>,
    },
    {
      headerName: t('codebook.county.columns.actions'),
      width: 120,
      sortable: false,
      filter: false,
      resizable: false,
      cellRenderer: (p: ICellRendererParams<CountyResponse>) => {
        const rec = p.data!
        return (
          <Space size={4}>
            <Tooltip title={t('codebook.county.actions.edit')}>
              <Button type="text" icon={<EditOutlined />} size="small" onClick={() => openEditModal(rec)} />
            </Tooltip>
            <Tooltip title={t(rec.isActive ? 'codebook.county.actions.deactivate' : 'codebook.county.actions.activate')}>
              <Button type="text" icon={rec.isActive ? <StopOutlined /> : <CheckOutlined />}
                size="small" danger={rec.isActive} onClick={() => confirmToggleActive(rec)} />
            </Tooltip>
            <Tooltip title={t('codebook.county.actions.delete')}>
              <Button type="text" icon={<DeleteOutlined />} size="small" danger onClick={() => confirmDelete(rec)} />
            </Tooltip>
          </Space>
        )
      },
    },
  ], [t, openEditModal, confirmToggleActive, confirmDelete])

  return (
    <CodebookLayout
      title={t('codebook.county.title')}
      extra={
        <Space>
          <Select
            allowClear
            placeholder={t('codebook.county.filter_country')}
            value={countryFilter}
            onChange={(v) => setCountryFilter(v ?? null)}
            options={countryOptions}
            style={{ minWidth: 160 }}
            size="small"
          />
          <Checkbox checked={onlyActive} onChange={e => setOnlyActive(e.target.checked)}>
            {t('common.only_active')}
          </Checkbox>
          <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal}>
            {t('codebook.county.addNew')}
          </Button>
        </Space>
      }
    >
      <AgGridWrapper<CountyResponse>
        columnDefs={columnDefs}
        rowData={filteredData}
        loading={loading}
        getRowId={(p) => p.data.id}
        getRowStyle={(p) => p.data?.isActive ? undefined : { opacity: 0.45 }}
        isDark={isDark}
      />

      <AppModal
        title={editingItem ? t('codebook.county.modal.editTitle') : t('codebook.county.modal.addTitle')}
        open={modalOpen}
        onOk={handleSave}
        onCancel={() => setModalOpen(false)}
        okText={t('common.save')}
        cancelText={t('common.cancel')}
        confirmLoading={saving}
        destroyOnClose
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item
            name="code"
            label={t('codebook.county.modal.code')}
            tooltip={t('codebook.county.modal.code_hint')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={20} style={{ textTransform: 'uppercase' }} />
          </Form.Item>
          <Form.Item
            name="nameHr"
            label={t('codebook.county.modal.nameHr')}
            rules={[{ required: true, message: t('common.required') }]}
          >
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="nameEn" label={t('codebook.county.modal.nameEn')}>
            <Input maxLength={200} />
          </Form.Item>
          <Form.Item name="countryId" label={t('codebook.county.modal.country')}>
            <Select allowClear options={countryOptions} />
          </Form.Item>
          <Form.Item name="ordinal" label={t('codebook.county.modal.ordinal')}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label={t('codebook.county.modal.isActive')} valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </AppModal>
    </CodebookLayout>
  )
}
